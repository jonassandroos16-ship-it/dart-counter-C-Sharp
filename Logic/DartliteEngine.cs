using dart_counter.Models;

namespace dart_counter.Logic;

public static class DartliteEngine
{
    private static readonly string[] EasyIds = { "goblin_scout", "goblin_brute", "orc_raider", "dark_mage", "royal_guard", "ice_wolf" };
    private static readonly string[] HardIds = { "frost_archer", "frost_knight", "vine_lasher", "spore_bloom", "thorn_spearman", "bloom_warden" };
    private static readonly string[] MinibossIds = { "warlord_malakar", "frost_knight", "bloom_warden" };
    private static readonly string[] BossIds = { "warlord_malakar", "ice_queen", "the_verdant_maw" };
    private static readonly Random _rng = new();

    public static bool IsMiniBossRound(int round) => round > 0 && round % 5 == 0 && round % 10 != 0;
    public static bool IsBossRound(int round) => round > 0 && round % 10 == 0;

    public static int XpForKill(Difficulty? enemyDifficulty) => enemyDifficulty switch
    {
        Difficulty.Boss => 100,
        Difficulty.Hard => 40,
        _ => 20,
    };

    public static int XpForBattleWin(int round)
    {
        if (IsBossRound(round)) return 200;
        if (IsMiniBossRound(round)) return 100;
        return 50;
    }

    public static double EnemyHpScale(int round) => Math.Min(3.0, 1 + Math.Max(0, round - 1) * 0.08);
    public static double EnemyAccScale(int round) => Math.Min(1.4, 1 + Math.Max(0, round - 1) * 0.015);
    public static double EnemyPrecScale(int round) => Math.Min(1.4, 1 + Math.Max(0, round - 1) * 0.015);

    public static DartliteRun StartRun(List<Player> players, Settings settings, bool cardMode = false)
    {
        var runPlayers = players.Select(p =>
        {
            var cfg = settings.PowerUpScaling;
            var startHealth = cfg.AttributeStartHealth > 0 ? cfg.AttributeStartHealth : 400;
            var h = p.Attributes?.Health > 0 ? p.Attributes!.Health : startHealth;
            var a = p.Attributes?.Armor ?? 0;
            var pw = p.Attributes?.Power ?? 0;
            return new DartliteRunPlayer
            {
                Id = p.Id, Name = p.Name, Color = p.Color,
                Hp = Math.Max(1, h), MaxHp = Math.Max(1, h),
                Power = Math.Max(0, pw), Armor = Math.Max(0, a),
                Cards = cardMode ? CardDeck.GetPlayerCards(p) : new(),
            };
        }).ToList();

        return new()
        {
            Round = 0,
            PlayerIds = players.Select(p => p.Id).ToList(),
            RunPlayers = runPlayers,
            Trinkets = new(),
            Pool = Trinkets.StarterPool.ToList(),
            Stats = new(),
            PlayerStats = players.Select(p => new DartlitePlayerRunStats { PlayerId = p.Id }).ToList(),
            Phase = DartlitePhase.Setup,
            CardMode = cardMode,
            PlayerChoices = players.Select(_ => (ChoiceOption?)null).ToList(),
        };
    }

    private static T Pick<T>(IList<T> arr) => arr[_rng.Next(arr.Count)];
    private static List<T> PickN<T>(IList<T> arr, int n)
    {
        var copy = arr.ToList();
        var result = new List<T>();
        for (int i = 0; i < n && copy.Count > 0; i++)
        {
            var idx = _rng.Next(copy.Count);
            result.Add(copy[idx]);
            copy.RemoveAt(idx);
        }
        return result;
    }

    public static CampaignLevel LevelForRound(int round)
    {
        if (IsBossRound(round))
        {
            var bossPool = round <= 10 ? new[] { "warlord_malakar" } : round <= 20 ? new[] { "ice_queen", "warlord_malakar" } : BossIds;
            return new() { LevelId = round, Name = $"Boss — Round {round}", IsBoss = true, Enemies = new() { Pick(bossPool) } };
        }
        if (IsMiniBossRound(round))
        {
            var miniPool = round <= 5 ? new[] { "warlord_malakar" } : round <= 15 ? MinibossIds : new[] { "frost_knight", "bloom_warden" };
            return new() { LevelId = round, Name = $"Mini-Boss — Round {round}", IsBoss = false, Enemies = new() { Pick(miniPool) } };
        }
        var count = Math.Min(3, 1 + round / 3);
        var pool = round <= 4 ? EasyIds : round <= 9 ? EasyIds.Concat(HardIds).ToArray() : HardIds;
        return new() { LevelId = round, Name = $"Round {round}", IsBoss = false, Enemies = PickN(pool, count) };
    }

    public static DartliteRun BeginRound(DartliteRun run, List<Player> players, Settings settings)
    {
        var round = run.Round + 1;
        var level = LevelForRound(round);
        var pseudoPlayers = run.RunPlayers.Select(rp =>
        {
            var orig = players.FirstOrDefault(p => p.Id == rp.Id) ?? new Player();
            orig.Attributes = new() { Health = rp.Hp, Armor = rp.Armor, Power = rp.Power, PointsAvailable = 0 };
            return orig;
        }).ToList();

        var battle = CoopEngine.StartBattle(level, pseudoPlayers, settings, "dartlite");
        var hpMult = EnemyHpScale(round);
        var accMult = EnemyAccScale(round);
        var precMult = EnemyPrecScale(round);
        foreach (var e in battle.Enemies)
        {
            e.MaxHp = (int)Math.Round(e.MaxHp * hpMult);
            e.Hp = e.MaxHp;
            e.Accuracy = Math.Min(0.95, e.Accuracy * accMult);
            e.Precision = Math.Min(0.95, e.Precision * precMult);
        }

        for (int i = 0; i < run.RunPlayers.Count; i++)
        {
            var rp = run.RunPlayers[i];
            if (rp.Trinkets.Contains("trk_overcharge"))
            {
                var idx = battle.Players.FindIndex(p => p.Id == rp.Id);
                if (idx >= 0)
                    battle.Players[idx].PowerUpCharge = Math.Min(settings.PowerUpScaling.ChargeMax, 40);
            }
        }

        return new DartliteRun
        {
            Round = round,
            PlayerIds = run.PlayerIds,
            RunPlayers = run.RunPlayers,
            Trinkets = run.Trinkets,
            Pool = run.Pool,
            Stats = run.Stats,
            PlayerStats = run.PlayerStats,
            Phase = DartlitePhase.Battle,
            CardMode = run.CardMode,
            Battle = battle,
            PlayerChoices = run.PlayerIds.Select(_ => (ChoiceOption?)null).ToList(),
            LastUnlockedTrinket = null,
            BossVictory = null,
            Log = run.Log,
        };
    }

    public static DartliteRun ResolveBattle(DartliteRun run, bool won)
    {
        if (run.Battle == null) return run;
        var battle = run.Battle;

        if (won)
        {
            var xp = XpForBattleWin(run.Round);
            int miniBosses = run.Stats.MiniBossesDefeated;
            int bosses = run.Stats.BossesDefeated;
            string? unlocked = null;

            if (IsMiniBossRound(run.Round))
            {
                miniBosses++;
                unlocked = Trinkets.NewlyUnlockedTrinket(miniBosses, bosses);
            }
            if (IsBossRound(run.Round))
            {
                bosses++;
                unlocked = Trinkets.NewlyUnlockedTrinket(miniBosses, bosses);
            }

            var newPool = Trinkets.AvailablePool(miniBosses, bosses);
            var partyHpAfter = Math.Max(0, battle.PartyHp);
            var runPlayers = run.RunPlayers.Select(rp => new DartliteRunPlayer
            {
                Id = rp.Id, Name = rp.Name, Color = rp.Color,
                Hp = partyHpAfter > 0 ? Math.Max(1, (int)Math.Round(partyHpAfter)) : rp.Hp,
                MaxHp = rp.MaxHp, Power = rp.Power, Armor = rp.Armor,
                Trinkets = rp.Trinkets, BonusHealth = rp.BonusHealth,
                BonusArmor = rp.BonusArmor, BonusPower = rp.BonusPower, Cards = rp.Cards,
            }).ToList();

            var playerStats = run.PlayerStats.Select(ps =>
            {
                var bp = battle.Players.FirstOrDefault(p => p.Id == ps.PlayerId);
                if (bp == null) return ps;
                return new DartlitePlayerRunStats
                {
                    PlayerId = ps.PlayerId,
                    Kills = ps.Kills + (bp.Kills ?? 0),
                    DamageDealt = ps.DamageDealt + (bp.DamageDealt ?? 0),
                    Rewards = ps.Rewards,
                    Trinkets = ps.Trinkets,
                };
            }).ToList();

            var stats = new DartliteRunStats
            {
                RoundsCleared = run.Stats.RoundsCleared + 1,
                EnemiesDefeated = run.Stats.EnemiesDefeated + battle.Stats.EnemiesDefeated,
                MiniBossesDefeated = miniBosses,
                BossesDefeated = bosses,
                DamageDealt = run.Stats.DamageDealt + battle.Stats.DamageDealt,
                XpGained = run.Stats.XpGained + xp,
                TrinketsCollected = run.Stats.TrinketsCollected,
            };

            if (IsBossRound(run.Round))
            {
                runPlayers = runPlayers.Select(rp => new DartliteRunPlayer
                {
                    Id = rp.Id, Name = rp.Name, Color = rp.Color,
                    Hp = rp.MaxHp, MaxHp = rp.MaxHp, Power = rp.Power, Armor = rp.Armor,
                    Trinkets = rp.Trinkets, BonusHealth = rp.BonusHealth,
                    BonusArmor = rp.BonusArmor, BonusPower = rp.BonusPower, Cards = rp.Cards,
                }).ToList();

                var bossName = battle.Enemies.Count > 0 ? battle.Enemies[0].Name : "Boss";
                var trinketOptions = Trinkets.BossTrinketOptions(bosses);
                var log = run.Log.Append($"Boss defeated on Round {run.Round} — {bossName} falls! Party healed to full.").ToList();

                return new DartliteRun
                {
                    Round = run.Round, PlayerIds = run.PlayerIds,
                    RunPlayers = runPlayers, Trinkets = run.Trinkets, Pool = newPool,
                    Stats = stats, PlayerStats = playerStats,
                    Phase = DartlitePhase.BossVictory,
                    CardMode = run.CardMode,
                    Battle = null, PendingChoice = null,
                    ChoicePlayerIdx = 0,
                    PlayerChoices = run.PlayerIds.Select(_ => (ChoiceOption?)null).ToList(),
                    LastUnlockedTrinket = unlocked,
                    BossVictory = new() { BossName = bossName, TrinketOptions = trinketOptions },
                    Log = log,
                };
            }

            var log2 = run.Log.Append($"Round {run.Round} cleared — +{xp} XP").ToList();
            return new DartliteRun
            {
                Round = run.Round, PlayerIds = run.PlayerIds,
                RunPlayers = runPlayers, Trinkets = run.Trinkets, Pool = newPool,
                Stats = stats, PlayerStats = playerStats,
                Phase = DartlitePhase.Choice,
                CardMode = run.CardMode,
                Battle = null,
                PendingChoice = GenerateChoices(run),
                ChoicePlayerIdx = 0,
                PlayerChoices = run.PlayerIds.Select(_ => (ChoiceOption?)null).ToList(),
                LastUnlockedTrinket = unlocked,
                BossVictory = null,
                Log = log2,
            };
        }

        return new DartliteRun
        {
            Round = run.Round, PlayerIds = run.PlayerIds,
            RunPlayers = run.RunPlayers, Trinkets = run.Trinkets, Pool = run.Pool,
            Stats = run.Stats, PlayerStats = run.PlayerStats,
            Phase = DartlitePhase.GameOver,
            CardMode = run.CardMode,
            Battle = null, PendingChoice = null,
            PlayerChoices = run.PlayerChoices,
            BossVictory = null,
            Log = run.Log,
        };
    }

    public static List<ChoiceOption> GenerateChoices(DartliteRun run)
    {
        var pool = run.Pool.Count > 0 ? run.Pool : Trinkets.StarterPool.ToList();
        var options = new List<ChoiceOption>
        {
            new() { Kind = ChoiceKind.Heal, Label = "Heal 20%", Desc = "Restore 20% of the party's max HP.", Icon = "❤️‍🩹" },
            new() { Kind = ChoiceKind.Stat, Label = "Gain a Stat", Desc = "+20 HP, +3% armor, or +4 power (random).", Icon = "📊" },
            new() { Kind = ChoiceKind.Trinket, Label = "Random Trinket", Desc = "Draw a random trinket from the available pool.", Icon = "🔮" },
        };
        if (pool.Count == 0)
            options[2] = new() { Kind = ChoiceKind.Heal, Label = "Heal 20%", Desc = "Restore 20% of max HP.", Icon = "❤️‍🩹" };
        return options;
    }

    public static DartliteRun ApplyPlayerChoice(DartliteRun run, ChoiceOption option)
    {
        var idx = run.ChoicePlayerIdx;
        var runPlayers = run.RunPlayers;
        var trinkets = run.Trinkets;
        var stats = run.Stats;
        var resolved = option;

        if (option.Kind == ChoiceKind.Heal)
        {
            var rp = runPlayers[idx];
            var healAmt = (int)Math.Round(rp.MaxHp * 0.2);
            runPlayers = runPlayers.Select((p, i) => i == idx ? new DartliteRunPlayer
            {
                Id = p.Id, Name = p.Name, Color = p.Color,
                Hp = Math.Min(p.MaxHp, p.Hp + healAmt), MaxHp = p.MaxHp,
                Power = p.Power, Armor = p.Armor, Trinkets = p.Trinkets,
                BonusHealth = p.BonusHealth, BonusArmor = p.BonusArmor, BonusPower = p.BonusPower, Cards = p.Cards,
            } : p).ToList();
            resolved = new() { Kind = ChoiceKind.Heal, Label = $"Heal {healAmt} HP", Desc = $"Restored {healAmt} HP ({rp.Name}).", Icon = option.Icon, Amount = healAmt };
        }
        else if (option.Kind == ChoiceKind.Stat)
        {
            var roll = _rng.NextDouble();
            string statName; int amount;
            if (roll < 0.4)
            {
                statName = "health"; amount = 20;
                runPlayers = runPlayers.Select((p, i) => i == idx ? new DartliteRunPlayer
                {
                    Id = p.Id, Name = p.Name, Color = p.Color,
                    Hp = p.Hp + 20, MaxHp = p.MaxHp + 20, Power = p.Power, Armor = p.Armor,
                    Trinkets = p.Trinkets, BonusHealth = p.BonusHealth + 20, BonusArmor = p.BonusArmor, BonusPower = p.BonusPower, Cards = p.Cards,
                } : p).ToList();
            }
            else if (roll < 0.7)
            {
                statName = "armor"; amount = 3;
                runPlayers = runPlayers.Select((p, i) => i == idx ? new DartliteRunPlayer
                {
                    Id = p.Id, Name = p.Name, Color = p.Color,
                    Hp = p.Hp, MaxHp = p.MaxHp, Power = p.Power, Armor = p.Armor + 3,
                    Trinkets = p.Trinkets, BonusHealth = p.BonusHealth, BonusArmor = p.BonusArmor + 3, BonusPower = p.BonusPower, Cards = p.Cards,
                } : p).ToList();
            }
            else
            {
                statName = "power"; amount = 4;
                runPlayers = runPlayers.Select((p, i) => i == idx ? new DartliteRunPlayer
                {
                    Id = p.Id, Name = p.Name, Color = p.Color,
                    Hp = p.Hp, MaxHp = p.MaxHp, Power = p.Power + 4, Armor = p.Armor,
                    Trinkets = p.Trinkets, BonusHealth = p.BonusHealth, BonusArmor = p.BonusArmor, BonusPower = p.BonusPower + 4, Cards = p.Cards,
                } : p).ToList();
            }
            var statLabel = statName == "health" ? $"+{amount} Max HP" : statName == "armor" ? $"+{amount}% Armor" : $"+{amount} Power";
            resolved = new() { Kind = ChoiceKind.Stat, Label = statLabel, Desc = $"Gained {statLabel}.", Icon = option.Icon, Stat = statName, Amount = amount };
        }
        else if (option.Kind == ChoiceKind.Trinket)
        {
            var pool = run.Pool.Count > 0 ? run.Pool : Trinkets.StarterPool.ToList();
            var id = option.TrinketId != null && pool.Contains(option.TrinketId) ? option.TrinketId : Pick(pool);
            runPlayers = runPlayers.Select((p, i) => i == idx ? new DartliteRunPlayer
            {
                Id = p.Id, Name = p.Name, Color = p.Color,
                Hp = p.Hp, MaxHp = p.MaxHp, Power = p.Power, Armor = p.Armor,
                Trinkets = p.Trinkets.Append(id!).ToList(),
                BonusHealth = p.BonusHealth, BonusArmor = p.BonusArmor, BonusPower = p.BonusPower, Cards = p.Cards,
            } : p).ToList();
            trinkets = trinkets.Append(id!).ToList();
            stats = new DartliteRunStats
            {
                RoundsCleared = stats.RoundsCleared,
                EnemiesDefeated = stats.EnemiesDefeated,
                MiniBossesDefeated = stats.MiniBossesDefeated,
                BossesDefeated = stats.BossesDefeated,
                DamageDealt = stats.DamageDealt,
                XpGained = stats.XpGained,
                TrinketsCollected = stats.TrinketsCollected.Append(id!).ToList(),
            };
            resolved = new() { Kind = ChoiceKind.Trinket, Label = option.Label, Desc = option.Desc, Icon = option.Icon, TrinketId = id };
        }

        var playerStats = run.PlayerStats.Select(ps =>
        {
            if (ps.PlayerId != run.PlayerIds[idx]) return ps;
            return new DartlitePlayerRunStats
            {
                PlayerId = ps.PlayerId,
                Kills = ps.Kills,
                DamageDealt = ps.DamageDealt,
                Rewards = ps.Rewards.Append(resolved).ToList(),
                Trinkets = resolved.TrinketId != null ? ps.Trinkets.Append(resolved.TrinketId).ToList() : ps.Trinkets,
            };
        }).ToList();

        var playerChoices = run.PlayerChoices.Select((c, i) => i == idx ? resolved : c).ToList();
        var nextIdx = idx + 1;
        var allChosen = nextIdx >= run.PlayerIds.Count;

        if (!allChosen)
        {
            return new DartliteRun
            {
                Round = run.Round, PlayerIds = run.PlayerIds,
                RunPlayers = runPlayers, Trinkets = trinkets, Pool = run.Pool,
                Stats = stats, PlayerStats = playerStats,
                Phase = DartlitePhase.Choice,
                CardMode = run.CardMode,
                Battle = null,
                PendingChoice = GenerateChoices(new DartliteRun { Pool = run.Pool, RunPlayers = runPlayers }),
                ChoicePlayerIdx = nextIdx,
                PlayerChoices = playerChoices,
                LastUnlockedTrinket = run.LastUnlockedTrinket,
                Log = run.Log,
            };
        }

        return new DartliteRun
        {
            Round = run.Round, PlayerIds = run.PlayerIds,
            RunPlayers = runPlayers, Trinkets = trinkets, Pool = run.Pool,
            Stats = stats, PlayerStats = playerStats,
            Phase = DartlitePhase.Reward,
            CardMode = run.CardMode,
            Battle = null, PendingChoice = null,
            ChoicePlayerIdx = idx,
            PlayerChoices = playerChoices,
            LastUnlockedTrinket = run.LastUnlockedTrinket,
            Log = run.Log,
        };
    }

    public static DartliteRun ApplyBossTrinketChoice(DartliteRun run, string trinketId)
    {
        if (run.BossVictory == null) return run;
        var def = Trinkets.GetTrinket(trinketId);
        if (def == null) return run;

        var runPlayers = run.RunPlayers.Select(rp => new DartliteRunPlayer
        {
            Id = rp.Id, Name = rp.Name, Color = rp.Color,
            Hp = rp.Hp, MaxHp = rp.MaxHp, Power = rp.Power, Armor = rp.Armor,
            Trinkets = rp.Trinkets.Append(trinketId).ToList(),
            BonusHealth = rp.BonusHealth, BonusArmor = rp.BonusArmor, BonusPower = rp.BonusPower, Cards = rp.Cards,
        }).ToList();

        void ApplyStat(Action<DartliteRunPlayer, int> setter, int amount)
        {
            for (int i = 0; i < runPlayers.Count; i++)
            {
                var rp = runPlayers[i];
                var np = new DartliteRunPlayer
                {
                    Id = rp.Id, Name = rp.Name, Color = rp.Color,
                    Hp = rp.Hp, MaxHp = rp.MaxHp, Power = rp.Power, Armor = rp.Armor,
                    Trinkets = rp.Trinkets,
                    BonusHealth = rp.BonusHealth, BonusArmor = rp.BonusArmor, BonusPower = rp.BonusPower, Cards = rp.Cards,
                };
                setter(np, amount);
                runPlayers[i] = np;
            }
        }

        if (trinketId == "trk_boss_warlords_crown") ApplyStat((p, v) => { p.Power += v; p.BonusPower += v; }, 25);
        else if (trinketId == "trk_boss_ice_crystal") ApplyStat((p, v) => { p.Armor += v; p.BonusArmor += v; }, 15);
        else if (trinketId == "trk_boss_verdant_seed") ApplyStat((p, v) => { p.MaxHp += v; p.Hp += v; p.BonusHealth += v; }, 200);
        else if (trinketId == "trk_boss_dragon_heart") ApplyStat((p, v) => { p.Power += v; p.BonusPower += v; }, 40);
        else if (trinketId == "trk_boss_frost_throne") ApplyStat((p, v) => { p.Armor += v; p.BonusArmor += v; }, 25);
        else if (trinketId == "trk_boss_maw_jaw") ApplyStat((p, v) => { p.MaxHp += v; p.Hp += v; p.BonusHealth += v; }, 400);
        else if (trinketId == "trk_boss_void_cloak") ApplyStat((p, v) => { p.Power += v; p.BonusPower += v; }, 60);
        else if (trinketId == "trk_boss_eternal_flame") ApplyStat((p, v) => { p.Armor += v; p.BonusArmor += v; }, 35);
        else if (trinketId == "trk_boss_titan_heart") ApplyStat((p, v) => { p.MaxHp += v; p.Hp += v; p.BonusHealth += v; }, 600);
        else if (trinketId == "trk_boss_godhand") ApplyStat((p, v) => { p.Power += v; p.BonusPower += v; }, 100);

        var trinkets = run.Trinkets.Append(trinketId).ToList();
        var stats = new DartliteRunStats
        {
            RoundsCleared = run.Stats.RoundsCleared,
            EnemiesDefeated = run.Stats.EnemiesDefeated,
            MiniBossesDefeated = run.Stats.MiniBossesDefeated,
            BossesDefeated = run.Stats.BossesDefeated,
            DamageDealt = run.Stats.DamageDealt,
            XpGained = run.Stats.XpGained,
            TrinketsCollected = run.Stats.TrinketsCollected.Append(trinketId).ToList(),
        };
        var playerStats = run.PlayerStats.Select(ps => new DartlitePlayerRunStats
        {
            PlayerId = ps.PlayerId, Kills = ps.Kills, DamageDealt = ps.DamageDealt,
            Rewards = ps.Rewards, Trinkets = ps.Trinkets.Append(trinketId).ToList(),
        }).ToList();
        var log = run.Log.Append($"Boss trinket chosen: {def.Name}").ToList();

        return new DartliteRun
        {
            Round = run.Round, PlayerIds = run.PlayerIds,
            RunPlayers = runPlayers, Trinkets = trinkets, Pool = run.Pool,
            Stats = stats, PlayerStats = playerStats,
            Phase = DartlitePhase.Reward,
            CardMode = run.CardMode,
            Battle = null, PendingChoice = null,
            ChoicePlayerIdx = 0,
            PlayerChoices = run.PlayerChoices,
            LastUnlockedTrinket = run.LastUnlockedTrinket,
            BossVictory = new() { BossName = run.BossVictory.BossName, TrinketOptions = run.BossVictory.TrinketOptions, ChosenTrinket = trinketId, ClaimedTrinket = trinketId },
            Log = log,
        };
    }

    public static bool HasTrinket(DartliteRun run, string id) => run.RunPlayers.Any(p => p.Trinkets.Contains(id));

    public static int PartyPowerBonus(DartliteRun run)
    {
        int bonus = 0;
        foreach (var p in run.RunPlayers)
        {
            if (p.Trinkets.Contains("trk_sharp_tip")) bonus += 5;
            if (p.Trinkets.Contains("trk_berserker") && p.Hp < p.MaxHp * 0.3) bonus += 15;
        }
        return bonus;
    }

    public static int PartyArmorBonus(DartliteRun run)
    {
        int bonus = 0;
        foreach (var p in run.RunPlayers)
            if (p.Trinkets.Contains("trk_thick_hide")) bonus += 8;
        return bonus;
    }

    public static int PartyMaxHpBonus(DartliteRun run)
    {
        int bonus = 0;
        foreach (var p in run.RunPlayers)
        {
            if (p.Trinkets.Contains("trk_vitality")) bonus += 60;
            if (p.Trinkets.Contains("trk_giants_belt")) bonus += (int)Math.Round(p.MaxHp * 0.5);
        }
        return bonus;
    }

    public static double EnemyAccuracyMultiplier(DartliteRun run)
    {
        double mult = 1;
        foreach (var p in run.RunPlayers)
            if (p.Trinkets.Contains("trk_quick_reflex")) mult -= 0.1;
        return Math.Max(0, mult);
    }

    public static double ChargeGainMultiplier(DartliteRun run)
    {
        double mult = 1;
        foreach (var p in run.RunPlayers)
            if (p.Trinkets.Contains("trk_lucky_penny")) mult += 0.3;
        return mult;
    }

    public static double XpMultiplier(DartliteRun run)
    {
        double mult = 1;
        foreach (var p in run.RunPlayers)
            if (p.Trinkets.Contains("trk_soul_harvest")) mult += 0.5;
        return mult;
    }

    public static bool ShouldPhoenixRevive(DartliteRun run) =>
        HasTrinket(run, "trk_phoenix_heart") && !run.Stats.TrinketsCollected.Contains("trk_phoenix_heart_used");

    public static DartliteRun ApplyPhoenixRevive(DartliteRun run)
    {
        var totalMax = run.RunPlayers.Sum(p => p.MaxHp);
        var reviveHp = (int)Math.Round(totalMax * 0.25);
        var perPlayer = (int)Math.Round((double)reviveHp / run.RunPlayers.Count);
        return new DartliteRun
        {
            Round = run.Round, PlayerIds = run.PlayerIds,
            RunPlayers = run.RunPlayers.Select(p => new DartliteRunPlayer
            {
                Id = p.Id, Name = p.Name, Color = p.Color,
                Hp = Math.Max(p.Hp, perPlayer), MaxHp = p.MaxHp, Power = p.Power, Armor = p.Armor,
                Trinkets = p.Trinkets, BonusHealth = p.BonusHealth, BonusArmor = p.BonusArmor, BonusPower = p.BonusPower, Cards = p.Cards,
            }).ToList(),
            Trinkets = run.Trinkets, Pool = run.Pool,
            Stats = new DartliteRunStats
            {
                RoundsCleared = run.Stats.RoundsCleared,
                EnemiesDefeated = run.Stats.EnemiesDefeated,
                MiniBossesDefeated = run.Stats.MiniBossesDefeated,
                BossesDefeated = run.Stats.BossesDefeated,
                DamageDealt = run.Stats.DamageDealt,
                XpGained = run.Stats.XpGained,
                TrinketsCollected = run.Stats.TrinketsCollected.Append("trk_phoenix_heart_used").ToList(),
            },
            PlayerStats = run.PlayerStats,
            Phase = run.Phase, CardMode = run.CardMode,
            Battle = run.Battle, PendingChoice = run.PendingChoice,
            ChoicePlayerIdx = run.ChoicePlayerIdx,
            PlayerChoices = run.PlayerChoices,
            LastUnlockedTrinket = run.LastUnlockedTrinket,
            BossVictory = run.BossVictory,
            Log = run.Log,
        };
    }

    public static void RecordDartliteRun(DartliteRun run, GameStateService state)
    {
        var seenTrinkets = run.Stats.TrinketsCollected.Distinct().Where(id => id != "trk_phoenix_heart_used").ToList();
        foreach (var p in state.Players)
        {
            if (!run.PlayerIds.Contains(p.Id)) continue;
            var cur = p.DartliteStats ?? new PlayerDartliteStats();
            p.DartliteStats = new()
            {
                Kills = cur.Kills + run.Stats.EnemiesDefeated,
                Battles = cur.Battles + run.Stats.RoundsCleared,
                MiniBossesDefeated = cur.MiniBossesDefeated + run.Stats.MiniBossesDefeated,
                BossesDefeated = cur.BossesDefeated + run.Stats.BossesDefeated,
                BestRound = Math.Max(cur.BestRound, run.Round),
                TotalXp = cur.TotalXp + run.Stats.XpGained,
                Runs = cur.Runs + 1,
                SeenTrinkets = cur.SeenTrinkets.Concat(seenTrinkets).Distinct().ToList(),
            };
        }
        _ = state.SetPlayers(state.Players);
    }
}
