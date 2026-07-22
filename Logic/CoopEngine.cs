namespace dart_counter.Logic;

using dart_counter.Models;

public static class CoopEngine
{
    static readonly int[] DartboardOrder = { 20, 1, 18, 4, 13, 6, 10, 15, 2, 17, 3, 19, 7, 16, 8, 11, 14, 9, 12, 5 };
    static readonly Random Rng = new();

    public static CampaignBattleState StartBattle(CampaignLevel level, List<Player> players, Settings settings, string chapterId = "crimson_vale")
    {
        var cfg = settings.PowerUpScaling;
        var chargeCap = cfg.ChargeMax;

        var party = players.Select(p =>
        {
            var equippedId = p.CoopProgress?.EquippedPassives?.FirstOrDefault();
            var startCharge = 0;
            return CoopParty.ToCoopPlayer(p, settings, Math.Max(0, Math.Min(chargeCap, startCharge)));
        }).ToList();

        var passiveBonus = CoopClasses.ComputePartyPassiveBonus(players);

        foreach (var cp in party)
        {
            cp.MaxHp = Math.Min(cfg.HealthMax, cp.MaxHp + passiveBonus.Health);
            cp.Hp = cp.MaxHp;
            cp.Power = Math.Min(cfg.PowerMax, cp.Power + passiveBonus.Power);
            cp.Armor = Math.Min(cfg.ArmorMax, cp.Armor + passiveBonus.Armor);
        }

        var baseAvg = players.Count > 0
            ? Math.Max(1, Math.Min(cfg.HealthMax, (int)Math.Round(players.Average(p =>
            {
                var h = p.Attributes?.Health;
                return h.HasValue ? Math.Max(1, Math.Min(cfg.HealthMax, h.Value)) : cfg.AttributeStartHealth;
            }))))
            : 1;
        var partyMaxHp = Math.Max(1, baseAvg + passiveBonus.Health);

        var enemies = level.Enemies.Select(defId =>
        {
            var def = EnemyDatabase.All[defId];
            return new ActiveEnemy
            {
                Id = CoopParty.NextInstanceId(defId),
                DefId = defId,
                Name = def.Name,
                Hp = def.MaxHp,
                MaxHp = def.MaxHp,
                Armor = def.Armor,
                Accuracy = def.Accuracy,
                Precision = def.Precision,
                Shields = def.Shields.Select(s => new ShieldLayer { Type = s.Type, TargetValue = s.TargetValue }).ToList(),
                Defeated = false
            };
        }).ToList();

        return new()
        {
            LevelId = level.LevelId,
            LevelName = level.Name,
            IsBoss = level.IsBoss,
            PartyHp = partyMaxHp,
            PartyMaxHp = partyMaxHp,
            Players = party,
            ChapterId = chapterId,
            Stats = new(),
            PlayerTurnIdx = 0,
            Darts = new(),
            Enemies = enemies,
            TargetIdx = 0,
            Phase = CoopPhase.Player,
            VisitNumber = 1,
            Outcome = CoopOutcome.Ongoing,
            PowerUpCharge = party.Count > 0 ? party[0].PowerUpCharge : 0,
            ResolvedDarts = new(),
            VisitEnemiesSnapshot = new(),
            PendingEnemyAttacks = new(),
            AppliedEnemyAttacks = new(),
            FrozenEnemiesThisRound = new(),
            PassiveBonus = passiveBonus
        };
    }

    public static CampaignDart MakeDart(int baseNum, int mult)
    {
        if (baseNum == 25)
            return new() { Value = mult == 2 ? 50 : 25, Label = mult == 2 ? "Bull" : "25", Base = 25, Mult = mult == 2 ? 2 : 1, IsDouble = mult == 2, IsBull = true };
        if (baseNum == 50)
            return new() { Value = 50, Label = "Bull", Base = 50, Mult = 2, IsDouble = true, IsBull = true };
        if (baseNum == 0)
            return new() { Value = 0, Label = "Miss", Base = 0, Mult = 1, IsDouble = false };
        var value = baseNum * mult;
        var label = (mult == 2 ? "D" : mult == 3 ? "T" : "") + baseNum;
        return new() { Value = value, Label = label, Base = baseNum, Mult = mult, IsDouble = mult == 2, IsBull = false };
    }

    public static CampaignBattleState AddDart(CampaignBattleState state, int baseNum, int mult, string? labelOverride = null, bool isBull = false, Settings? settings = null)
    {
        if (state.Phase != CoopPhase.Player) return state;
        if (state.Darts.Count >= 3) return state;

        int value;
        string label;
        if (isBull) { value = 50; label = "Bull"; }
        else if (baseNum == 25) { value = mult == 2 ? 50 : 25; label = mult == 2 ? "Bull" : "25"; }
        else if (baseNum == 0) { value = 0; label = "Miss"; }
        else { value = baseNum * mult; label = (mult == 2 ? "D" : mult == 3 ? "T" : "") + baseNum; }

        var dart = new CampaignDart
        {
            Value = value,
            Label = labelOverride ?? label,
            Base = baseNum,
            Mult = isBull ? 2 : (baseNum == 25 && value == 50 ? 2 : mult),
            IsDouble = isBull || (baseNum == 25 && value == 50) || mult == 2,
            IsBull = isBull || baseNum == 25
        };

        var phantomDarts = state.PhantomDarts;
        if (phantomDarts > 0 && baseNum != 0)
        {
            dart = new() { Value = 50, Label = "👻 Bull", Base = 50, Mult = 2, IsDouble = true, IsBull = true };
            phantomDarts--;
        }

        var chargeCap = settings?.PowerUpScaling.ChargeMax ?? 100;
        var gained = settings != null ? ChargeFromDart(dart, settings) : 0;
        var throwerIdx = state.PlayerTurnIdx;
        var players = state.Players.Select((p, i) => i == throwerIdx
            ? new CoopPlayer { Id = p.Id, Name = p.Name, Color = p.Color, Hp = p.Hp, MaxHp = p.MaxHp, Power = p.Power, Armor = p.Armor, Buffs = p.Buffs, PowerUpCharge = Math.Min(chargeCap, p.PowerUpCharge + gained), ClassId = p.ClassId, Kills = p.Kills, DamageDealt = p.DamageDealt }
            : p).ToList();
        var powerUpCharge = players[throwerIdx].PowerUpCharge;

        var visitEnemiesSnapshot = state.Darts.Count == 0
            ? state.Enemies.Select(e => CloneEnemy(e)).ToList()
            : state.VisitEnemiesSnapshot;

        var thrower = players[throwerIdx];
        if (thrower == null)
            return CloneWith(state, players: players, darts: state.Darts.Append(dart).ToList(), phantomDarts: phantomDarts, visitEnemiesSnapshot: visitEnemiesSnapshot, powerUpCharge: powerUpCharge);

        var power = EffectivePower(thrower);

        var targetIdx = state.TargetIdx;
        var target = state.Enemies[targetIdx];
        if (target == null || target.Defeated)
        {
            var firstAlive = state.Enemies.FindIndex(e => !e.Defeated);
            if (firstAlive < 0)
                return CloneWith(state, players: players, darts: state.Darts.Append(dart).ToList(), phantomDarts: phantomDarts, visitEnemiesSnapshot: visitEnemiesSnapshot, powerUpCharge: powerUpCharge);
            targetIdx = firstAlive;
            target = state.Enemies[targetIdx];
        }

        var enemies = state.Enemies.Select(e => CloneEnemy(e)).ToList();
        var t = enemies[targetIdx];
        ResolvedDart step;

        if (t.Shields.Count > 0)
        {
            var shield = t.Shields[0];
            if (CoopShields.DartMatchesShield(dart, shield))
            {
                t.Shields.RemoveAt(0);
                step = new() { Dart = dart, Damage = 0, Kind = "shield_break", ShieldTarget = CoopShields.DescribeShield(shield), EnemyId = t.Id, EnemyName = t.Name, HpAfter = t.Hp, AttackerPower = power, TargetArmor = t.Armor, Vulnerable = t.VulnerableTurns > 0 };
            }
            else
            {
                step = new() { Dart = dart, Damage = 0, Kind = "miss", EnemyId = t.Id, EnemyName = t.Name, HpAfter = t.Hp, AttackerPower = power, TargetArmor = t.Armor, Vulnerable = t.VulnerableTurns > 0 };
            }
        }
        else
        {
            var dmg = ComputePlayerDartDamage(dart, power, t.Armor);
            var vulnerable = t.VulnerableTurns > 0;
            var finalDmg = vulnerable ? (int)Math.Round(dmg * 1.5) : dmg;
            t.Hp = Math.Max(0, t.Hp - finalDmg);
            if (t.Hp <= 0) t.Defeated = true;
            step = new() { Dart = dart, Damage = finalDmg, Kind = t.Defeated ? "defeated" : "damage", EnemyId = t.Id, EnemyName = t.Name, HpAfter = t.Hp, AttackerPower = power, TargetArmor = t.Armor, Vulnerable = vulnerable };
        }

        var anyAlive = enemies.Any(e => !e.Defeated);
        var outcome = !anyAlive ? CoopOutcome.Victory : state.Outcome;

        var dartDamage = step.Kind == "damage" || step.Kind == "defeated" ? step.Damage : 0;
        var dartKill = step.Kind == "defeated" ? 1 : 0;
        var playersWithStats = players.Select((p, i) => i == throwerIdx
            ? new CoopPlayer { Id = p.Id, Name = p.Name, Color = p.Color, Hp = p.Hp, MaxHp = p.MaxHp, Power = p.Power, Armor = p.Armor, Buffs = p.Buffs, PowerUpCharge = p.PowerUpCharge, ClassId = p.ClassId, Kills = p.Kills + dartKill, DamageDealt = p.DamageDealt + dartDamage }
            : p).ToList();

        return new()
        {
            LevelId = state.LevelId, LevelName = state.LevelName, IsBoss = state.IsBoss,
            PartyHp = state.PartyHp, PartyMaxHp = state.PartyMaxHp,
            Players = playersWithStats, ChapterId = state.ChapterId,
            Stats = new() { VisitsUsed = state.Stats.VisitsUsed, DartsThrown = state.Stats.DartsThrown + 1, DamageDealt = state.Stats.DamageDealt + dartDamage, EnemiesDefeated = state.Stats.EnemiesDefeated + dartKill, PowerUpsUsed = state.Stats.PowerUpsUsed, PartyHpLost = state.Stats.PartyHpLost },
            PlayerTurnIdx = state.PlayerTurnIdx, Darts = state.Darts.Append(dart).ToList(),
            Enemies = enemies, TargetIdx = targetIdx, Phase = state.Phase,
            VisitNumber = state.VisitNumber, Outcome = outcome, PowerUpCharge = powerUpCharge,
            ResolvedDarts = state.ResolvedDarts.Append(step).ToList(),
            VisitEnemiesSnapshot = visitEnemiesSnapshot,
            PendingEnemyAttacks = state.PendingEnemyAttacks, AppliedEnemyAttacks = state.AppliedEnemyAttacks,
            AwaitContinue = state.AwaitContinue, PhantomDarts = phantomDarts,
            FrozenEnemiesThisRound = state.FrozenEnemiesThisRound, PassiveBonus = state.PassiveBonus
        };
    }

    public static CampaignBattleState UndoDart(CampaignBattleState state, Settings? settings)
    {
        if (state.Darts.Count == 0) return state;
        var enemies = state.VisitEnemiesSnapshot.Count > 0
            ? state.VisitEnemiesSnapshot.Select(e => CloneEnemy(e)).ToList()
            : state.Enemies;
        var resolvedDarts = state.ResolvedDarts.Take(state.ResolvedDarts.Count - 1).ToList();
        var darts = state.Darts.Take(state.Darts.Count - 1).ToList();
        var visitEnemiesSnapshot = darts.Count == 0 ? new List<ActiveEnemy>() : state.VisitEnemiesSnapshot;
        var anyAlive = enemies.Any(e => !e.Defeated);
        var outcome = !anyAlive ? CoopOutcome.Victory : CoopOutcome.Ongoing;

        var undoneDart = state.Darts[state.Darts.Count - 1];
        var revert = settings != null ? ChargeFromDart(undoneDart, settings) : 0;
        var throwerIdx = state.PlayerTurnIdx;
        var players = state.Players.Select((p, i) => i == throwerIdx
            ? new CoopPlayer { Id = p.Id, Name = p.Name, Color = p.Color, Hp = p.Hp, MaxHp = p.MaxHp, Power = p.Power, Armor = p.Armor, Buffs = p.Buffs, PowerUpCharge = Math.Max(0, p.PowerUpCharge - revert), ClassId = p.ClassId, Kills = p.Kills, DamageDealt = p.DamageDealt }
            : p).ToList();
        var powerUpCharge = Math.Max(0, state.PowerUpCharge - revert);

        return new()
        {
            LevelId = state.LevelId, LevelName = state.LevelName, IsBoss = state.IsBoss,
            PartyHp = state.PartyHp, PartyMaxHp = state.PartyMaxHp,
            Players = players, ChapterId = state.ChapterId,
            Stats = state.Stats, PlayerTurnIdx = state.PlayerTurnIdx,
            Darts = darts, Enemies = enemies, TargetIdx = state.TargetIdx,
            Phase = state.Phase, VisitNumber = state.VisitNumber,
            Outcome = outcome, PowerUpCharge = powerUpCharge,
            ResolvedDarts = resolvedDarts, VisitEnemiesSnapshot = visitEnemiesSnapshot,
            PendingEnemyAttacks = state.PendingEnemyAttacks, AppliedEnemyAttacks = state.AppliedEnemyAttacks,
            AwaitContinue = state.AwaitContinue, PhantomDarts = state.PhantomDarts,
            FrozenEnemiesThisRound = state.FrozenEnemiesThisRound, PassiveBonus = state.PassiveBonus
        };
    }

    public static CampaignBattleState SetTarget(CampaignBattleState state, string enemyId)
    {
        var idx = state.Enemies.FindIndex(e => e.Id == enemyId);
        if (idx < 0) return state;
        return CloneWith(state, targetIdx: idx);
    }

    public static int EffectivePower(CoopPlayer player)
    {
        var buff = player.Buffs.Where(b => b.Kind == "power").Sum(b => b.Amount);
        return Math.Max(0, player.Power + buff);
    }

    public static int ComputePlayerDartDamage(CampaignDart dart, int attackerPower, int targetArmor)
    {
        if (dart.Value <= 0) return 0;
        var baseDmg = Math.Max(0, dart.Value + attackerPower);
        var armorPct = Math.Max(0, targetArmor);
        var mitigated = baseDmg * (1 - armorPct / 100.0);
        return Math.Max(1, (int)Math.Round(mitigated));
    }

    public static int ChargeFromDart(CampaignDart dart, Settings settings)
    {
        var cfg = settings.PowerUpScaling;
        int c = 0;
        var isBull = dart.Value == 50 || dart.Value == 25;
        if (isBull) c += (int)cfg.ChargePerBull;
        else if (dart.Mult == 3) c += (int)cfg.ChargePerTriple;
        else if (dart.Mult == 2 || dart.IsDouble) c += (int)cfg.ChargePerDouble;
        c += (int)(dart.Value * cfg.ChargePerScorePoint);
        return c;
    }

    public static CampaignBattleState ResolvePlayerVisit(CampaignBattleState state)
    {
        if (state.Phase != CoopPhase.Player) return state;
        if (state.Darts.Count == 0) return state;
        return AdvanceAfterPlayerVisit(new()
        {
            LevelId = state.LevelId, LevelName = state.LevelName, IsBoss = state.IsBoss,
            PartyHp = state.PartyHp, PartyMaxHp = state.PartyMaxHp,
            Players = state.Players, ChapterId = state.ChapterId,
            Stats = new() { VisitsUsed = state.Stats.VisitsUsed + 1, DartsThrown = state.Stats.DartsThrown, DamageDealt = state.Stats.DamageDealt, EnemiesDefeated = state.Stats.EnemiesDefeated, PowerUpsUsed = state.Stats.PowerUpsUsed, PartyHpLost = state.Stats.PartyHpLost },
            PlayerTurnIdx = state.PlayerTurnIdx, Darts = new(), Enemies = state.Enemies,
            TargetIdx = state.TargetIdx, Phase = state.Phase, VisitNumber = state.VisitNumber,
            Outcome = state.Outcome, PowerUpCharge = state.PowerUpCharge,
            ResolvedDarts = new(), VisitEnemiesSnapshot = new(),
            PendingEnemyAttacks = state.PendingEnemyAttacks, AppliedEnemyAttacks = state.AppliedEnemyAttacks,
            AwaitContinue = false, PhantomDarts = state.PhantomDarts,
            FrozenEnemiesThisRound = state.FrozenEnemiesThisRound, PassiveBonus = state.PassiveBonus
        });
    }

    static CampaignBattleState AdvanceAfterPlayerVisit(CampaignBattleState state)
    {
        if (state.Outcome == CoopOutcome.Victory)
            return CloneWith(state, darts: new(), resolvedDarts: new(), visitEnemiesSnapshot: new(), awaitContinue: false);
        var nextIdx = state.PlayerTurnIdx + 1;
        if (nextIdx < state.Players.Count)
            return CloneWith(state, playerTurnIdx: nextIdx, darts: new(), resolvedDarts: new(), visitEnemiesSnapshot: new(), awaitContinue: false);
        return CloneWith(state, phase: CoopPhase.Enemy, darts: new(), resolvedDarts: new(), visitEnemiesSnapshot: new(), awaitContinue: false);
    }

    // ── Enemy AI ──

    public static double EffectiveAccuracy(ActiveEnemy enemy) =>
        enemy.DistractedTurns > 0 ? Math.Max(0, enemy.Accuracy - enemy.DistractAmount) : enemy.Accuracy;
    public static double EffectivePrecision(ActiveEnemy enemy) =>
        enemy.DistractedTurns > 0 ? Math.Max(0, enemy.Precision - enemy.DistractAmount) : enemy.Precision;

    public static CampaignDart SimulateEnemyDart(ActiveEnemy enemy)
    {
        const int intendedBase = 20;
        const int intendedMult = 3;
        var hit = Rng.NextDouble() < EffectiveAccuracy(enemy);
        int baseNum = intendedBase;
        int mult = intendedMult;
        if (!hit)
        {
            if (Rng.NextDouble() < EffectivePrecision(enemy))
            {
                var neighbors = CoopShields.NeighborsOf(intendedBase);
                baseNum = neighbors.Length > 0 ? neighbors[Rng.Next(neighbors.Length)] : intendedBase;
            }
            else
            {
                baseNum = DartboardOrder[Rng.Next(DartboardOrder.Length)];
            }
            var r = Rng.NextDouble();
            mult = r < 0.1 ? 3 : r < 0.25 ? 2 : 1;
        }
        return MakeDart(baseNum, mult);
    }

    public static CampaignBattleState PrepareEnemyTurn(CampaignBattleState state)
    {
        if (state.Phase != CoopPhase.Enemy) return state;
        var steps = new List<EnemyAttackStep>();
        var partyHp = state.PartyHp;
        var frozenEnemiesThisRound = new List<FrozenEnemyInfo>();
        var enemies = state.Enemies.Select(e => CloneEnemy(e)).ToList();

        foreach (var enemy in enemies)
        {
            if (enemy.Defeated) continue;
            if (enemy.FrozenTurns > 0)
            {
                frozenEnemiesThisRound.Add(new() { Id = enemy.Id, Name = enemy.Name, FrozenTurns = enemy.FrozenTurns });
                enemy.FrozenTurns--;
                continue;
            }
            for (int i = 0; i < 3; i++)
            {
                var dart = SimulateEnemyDart(enemy);
                var dmg = Math.Max(0, dart.Value);
                partyHp = Math.Max(0, partyHp - dmg);
                steps.Add(new() { EnemyId = enemy.Id, EnemyName = enemy.Name, Dart = dart, Damage = dmg, PartyHpAfter = partyHp });
            }
        }

        return CloneWith(state, enemies: enemies, pendingEnemyAttacks: steps, appliedEnemyAttacks: new(), frozenEnemiesThisRound: frozenEnemiesThisRound, awaitContinue: true, partyHp: partyHp);
    }

    public static CampaignBattleState ApplyNextEnemyAttack(CampaignBattleState state)
    {
        if (state.PendingEnemyAttacks.Count == 0)
            return FinishEnemyTurn(state);

        var step = state.PendingEnemyAttacks[0];
        var rest = state.PendingEnemyAttacks.Skip(1).ToList();
        var partyHpLost = state.Stats.PartyHpLost + (step.Damage > 0 ? step.Damage : 0);

        var next = CloneWith(state, partyHp: step.PartyHpAfter, pendingEnemyAttacks: rest, appliedEnemyAttacks: state.AppliedEnemyAttacks.Append(step).ToList(), awaitContinue: rest.Count > 0,
            stats: new() { VisitsUsed = state.Stats.VisitsUsed, DartsThrown = state.Stats.DartsThrown, DamageDealt = state.Stats.DamageDealt, EnemiesDefeated = state.Stats.EnemiesDefeated, PowerUpsUsed = state.Stats.PowerUpsUsed, PartyHpLost = partyHpLost });

        if (next.PartyHp <= 0)
            return CloneWith(next, outcome: CoopOutcome.Defeat, phase: CoopPhase.Player, pendingEnemyAttacks: new(), appliedEnemyAttacks: new(), awaitContinue: false);
        if (rest.Count == 0)
            return FinishEnemyTurn(CloneWith(next, awaitContinue: false));
        return next;
    }

    public static CampaignBattleState FinishEnemyTurn(CampaignBattleState state)
    {
        if (state.PartyHp <= 0)
            return CloneWith(state, outcome: CoopOutcome.Defeat, phase: CoopPhase.Player, awaitContinue: false);

        var players = state.Players.Select(p => new CoopPlayer
        {
            Id = p.Id, Name = p.Name, Color = p.Color, Hp = p.Hp, MaxHp = p.MaxHp, Power = p.Power, Armor = p.Armor,
            Buffs = p.Buffs.Select(b => new PlayerBuff { Id = b.Id, Kind = b.Kind, Amount = b.Amount, TurnsLeft = b.TurnsLeft - 1, Source = b.Source }).Where(b => b.TurnsLeft > 0).ToList(),
            PowerUpCharge = p.PowerUpCharge, ClassId = p.ClassId, Kills = p.Kills, DamageDealt = p.DamageDealt
        }).ToList();

        var enemies = state.Enemies.Select(e => CloneEnemyWith(e, vulnerableTurns: Math.Max(0, e.VulnerableTurns - 1), distractedTurns: Math.Max(0, e.DistractedTurns - 1), distractAmount: e.DistractedTurns - 1 > 0 ? e.DistractAmount : 0)).ToList();

        return CloneWith(state, players: players, enemies: enemies, phase: CoopPhase.Player, playerTurnIdx: 0, darts: new(), resolvedDarts: new(), visitEnemiesSnapshot: new(),
            pendingEnemyAttacks: new(), appliedEnemyAttacks: new(), frozenEnemiesThisRound: new(), visitNumber: state.VisitNumber + 1, awaitContinue: false);
    }

    // ── Helpers ──

    public static CampaignBattleState CloneWithPublic(CampaignBattleState s, bool? awaitContinue = null) => CloneWith(s, awaitContinue: awaitContinue);

    static ActiveEnemy CloneEnemy(ActiveEnemy e) => new()
    {
        Id = e.Id, DefId = e.DefId, Name = e.Name, Hp = e.Hp, MaxHp = e.MaxHp, Armor = e.Armor,
        Accuracy = e.Accuracy, Precision = e.Precision, Shields = e.Shields.Select(s => new ShieldLayer { Type = s.Type, TargetValue = s.TargetValue }).ToList(),
        Defeated = e.Defeated, FrozenTurns = e.FrozenTurns, VulnerableTurns = e.VulnerableTurns, DistractedTurns = e.DistractedTurns, DistractAmount = e.DistractAmount
    };

    static ActiveEnemy CloneEnemyWith(ActiveEnemy e, int? vulnerableTurns = null, int? distractedTurns = null, double? distractAmount = null) => new()
    {
        Id = e.Id, DefId = e.DefId, Name = e.Name, Hp = e.Hp, MaxHp = e.MaxHp, Armor = e.Armor,
        Accuracy = e.Accuracy, Precision = e.Precision, Shields = e.Shields.Select(s => new ShieldLayer { Type = s.Type, TargetValue = s.TargetValue }).ToList(),
        Defeated = e.Defeated, FrozenTurns = e.FrozenTurns,
        VulnerableTurns = vulnerableTurns ?? e.VulnerableTurns,
        DistractedTurns = distractedTurns ?? e.DistractedTurns,
        DistractAmount = distractAmount ?? e.DistractAmount
    };

    static CampaignBattleState CloneWith(CampaignBattleState s,
        List<CoopPlayer>? players = null, List<CampaignDart>? darts = null, List<ActiveEnemy>? enemies = null,
        int? targetIdx = null, CoopPhase? phase = null, int? playerTurnIdx = null, int? visitNumber = null,
        CoopOutcome? outcome = null, int? powerUpCharge = null, List<ResolvedDart>? resolvedDarts = null,
        List<ActiveEnemy>? visitEnemiesSnapshot = null, List<EnemyAttackStep>? pendingEnemyAttacks = null,
        List<EnemyAttackStep>? appliedEnemyAttacks = null, bool? awaitContinue = null, int? phantomDarts = null,
        List<FrozenEnemyInfo>? frozenEnemiesThisRound = null, int? partyHp = null, BattleStats? stats = null,
        PartyPassiveBonus? passiveBonus = null) => new()
    {
        LevelId = s.LevelId, LevelName = s.LevelName, IsBoss = s.IsBoss,
        PartyHp = partyHp ?? s.PartyHp, PartyMaxHp = s.PartyMaxHp,
        Players = players ?? s.Players, ChapterId = s.ChapterId,
        Stats = stats ?? s.Stats, PlayerTurnIdx = playerTurnIdx ?? s.PlayerTurnIdx,
        Darts = darts ?? s.Darts, Enemies = enemies ?? s.Enemies,
        TargetIdx = targetIdx ?? s.TargetIdx, Phase = phase ?? s.Phase,
        VisitNumber = visitNumber ?? s.VisitNumber, Outcome = outcome ?? s.Outcome,
        PowerUpCharge = powerUpCharge ?? s.PowerUpCharge,
        ResolvedDarts = resolvedDarts ?? s.ResolvedDarts,
        VisitEnemiesSnapshot = visitEnemiesSnapshot ?? s.VisitEnemiesSnapshot,
        PendingEnemyAttacks = pendingEnemyAttacks ?? s.PendingEnemyAttacks,
        AppliedEnemyAttacks = appliedEnemyAttacks ?? s.AppliedEnemyAttacks,
        AwaitContinue = awaitContinue ?? s.AwaitContinue,
        PhantomDarts = phantomDarts ?? s.PhantomDarts,
        FrozenEnemiesThisRound = frozenEnemiesThisRound ?? s.FrozenEnemiesThisRound,
        PassiveBonus = passiveBonus ?? s.PassiveBonus
    };
}
