namespace dart_counter.Logic;

using dart_counter.Models;

public static class CoopActions
{
    const double FocusBuffDistractAmount = 0.2;
    const int FocusBuffTurns = 3;

    public static bool CanActivate(CampaignBattleState state, string id)
    {
        if (state.Phase != CoopPhase.Player) return false;
        if (state.Darts.Count > 0) return false;
        var def = CoopPowerUps.Get(id);
        if (def == null) return false;
        var thrower = state.Players[state.PlayerTurnIdx];
        if (thrower == null) return false;
        return thrower.PowerUpCharge >= def.Cost;
    }

    public static CampaignBattleState Activate(CampaignBattleState state, string id)
    {
        if (!CanActivate(state, id)) return state;
        var def = CoopPowerUps.Get(id)!;
        var throwerIdx = state.PlayerTurnIdx;
        var thrower = state.Players[throwerIdx];
        var newCharge = thrower.PowerUpCharge - def.Cost;
        var players = state.Players.Select((p, i) => i == throwerIdx
            ? ClonePlayer(p, powerUpCharge: newCharge)
            : p).ToList();
        var powerUpCharge = newCharge;

        var defeatedBefore = state.Enemies.Count(e => e.Defeated);
        var next = ApplyPowerUp(CloneWith(state, players: players), id, thrower, powerUpCharge);
        var anyAlive = next.Enemies.Any(e => !e.Defeated);
        var outcome = !anyAlive ? CoopOutcome.Victory : next.Outcome;
        var defeatedAfter = next.Enemies.Count(e => e.Defeated);
        var newlyDefeated = Math.Max(0, defeatedAfter - defeatedBefore);

        return CloneWith(next, outcome: outcome, stats: new()
        {
            VisitsUsed = next.Stats.VisitsUsed, DartsThrown = next.Stats.DartsThrown,
            DamageDealt = next.Stats.DamageDealt, EnemiesDefeated = next.Stats.EnemiesDefeated + newlyDefeated,
            PowerUpsUsed = next.Stats.PowerUpsUsed + 1, PartyHpLost = next.Stats.PartyHpLost
        });
    }

    static CampaignBattleState ApplyPowerUp(CampaignBattleState state, string id, CoopPlayer thrower, int charge)
    {
        switch (id)
        {
            case "coop_heal":
                return CloneWith(state, partyHp: Math.Min(state.PartyMaxHp, state.PartyHp + 80), powerUpCharge: charge);
            case "coop_buff_power":
                {
                    var buffId = $"power_{DateTime.UtcNow.Ticks}";
                    var players = state.Players.Select(p => ClonePlayer(p, buffs: p.Buffs.Append(new() { Id = buffId, Kind = "power", Amount = 10, TurnsLeft = 3, Source = thrower.Id }).ToList())).ToList();
                    return CloneWith(state, players: players, powerUpCharge: charge);
                }
            case "coop_buff_acc":
                {
                    var enemies = state.Enemies.Select(e => e.Defeated ? e : CloneEnemy(e, distractedTurns: FocusBuffTurns, distractAmount: FocusBuffDistractAmount)).ToList();
                    return CloneWith(state, enemies: enemies, powerUpCharge: charge);
                }
            case "coop_freeze":
                {
                    var enemies = state.Enemies.Select(e => e.Defeated ? e : CloneEnemy(e, frozenTurns: 2)).ToList();
                    return CloneWith(state, enemies: enemies, powerUpCharge: charge);
                }
            case "coop_shield":
                return CloneWith(state, partyHp: Math.Min(state.PartyMaxHp, state.PartyHp + 40), powerUpCharge: charge);
            case "coop_meteor":
                {
                    var enemies = state.Enemies.Select(e => e.Defeated ? e : DamageEnemy(e, 60)).ToList();
                    return CloneWith(state, enemies: enemies, powerUpCharge: charge);
                }
            case "coop_phantom":
                return CloneWith(state, phantomDarts: 3, powerUpCharge: charge);
            case "coop_time_warp":
                {
                    var enemies = state.Enemies.Select(e => e.Defeated ? e : CloneEnemy(e, vulnerableTurns: 3)).ToList();
                    return CloneWith(state, enemies: enemies, powerUpCharge: charge);
                }
            case "coop_ressurect":
                {
                    var enemies = state.Enemies.Select(e => CloneEnemy(e, shields: new())).ToList();
                    return CloneWith(state, partyHp: state.PartyMaxHp, enemies: enemies, powerUpCharge: charge);
                }
            case "coop_apocalypse":
                {
                    var enemies = state.Enemies.Select(e => e.Defeated ? e : DamageEnemy(e, 150, freezeTurns: 2, clearShields: true)).ToList();
                    return CloneWith(state, enemies: enemies, partyHp: state.PartyMaxHp, powerUpCharge: charge);
                }
            case "coop_blizzard":
                {
                    var enemies = state.Enemies.Select(e => e.Defeated ? e : DamageEnemy(e, 45, freezeTurns: 1)).ToList();
                    return CloneWith(state, enemies: enemies, powerUpCharge: charge);
                }
            case "coop_frostbite":
                {
                    var enemies = state.Enemies.Select(e => e.Defeated ? e : DamageEnemy(e, 40, distractTurns: FocusBuffTurns, distractAmount: 0.25)).ToList();
                    return CloneWith(state, enemies: enemies, powerUpCharge: charge);
                }
            case "coop_ice_lance":
                return SingleTargetNuke(state, 120, charge);
            case "coop_winter_veil":
                {
                    var enemies = state.Enemies.Select(e => CloneEnemy(e, shields: new())).ToList();
                    return CloneWith(state, partyHp: Math.Min(state.PartyMaxHp, state.PartyHp + 60), enemies: enemies, powerUpCharge: charge);
                }
            case "coop_glacial_doom":
                {
                    var enemies = state.Enemies.Select(e => e.Defeated ? e : DamageEnemy(e, 180, freezeTurns: 3, clearShields: true)).ToList();
                    return CloneWith(state, enemies: enemies, partyHp: state.PartyMaxHp, powerUpCharge: charge);
                }
            case "coop_vine_grasp":
                {
                    var enemies = state.Enemies.Select(e => e.Defeated ? e : DamageEnemy(e, 50, freezeTurns: 1)).ToList();
                    return CloneWith(state, enemies: enemies, powerUpCharge: charge);
                }
            case "coop_spore_burst":
                {
                    var enemies = state.Enemies.Select(e => e.Defeated ? e : DamageEnemy(e, 60, distractTurns: FocusBuffTurns, distractAmount: 0.30)).ToList();
                    return CloneWith(state, enemies: enemies, powerUpCharge: charge);
                }
            case "coop_thorn_lance":
                return SingleTargetNuke(state, 160, charge);
            case "coop_verdant_bloom":
                {
                    var enemies = state.Enemies.Select(e => CloneEnemy(e, shields: new())).ToList();
                    var buffId = $"power_{DateTime.UtcNow.Ticks}";
                    var players = state.Players.Select(p => ClonePlayer(p, buffs: p.Buffs.Append(new() { Id = buffId, Kind = "power", Amount = 5, TurnsLeft = 3, Source = thrower.Id }).ToList())).ToList();
                    return CloneWith(state, partyHp: Math.Min(state.PartyMaxHp, state.PartyHp + 100), enemies: enemies, players: players, powerUpCharge: charge);
                }
            case "coop_heart_of_maw":
                {
                    var enemies = state.Enemies.Select(e => e.Defeated ? e : DamageEnemy(e, 220, freezeTurns: 3, clearShields: true)).ToList();
                    return CloneWith(state, enemies: enemies, partyHp: state.PartyMaxHp, powerUpCharge: charge);
                }
        }
        return state;
    }

    static CampaignBattleState SingleTargetNuke(CampaignBattleState state, int damage, int charge)
    {
        var targetIdx = state.TargetIdx;
        var target = state.Enemies[targetIdx];
        if (target == null || target.Defeated)
        {
            var firstAlive = state.Enemies.FindIndex(e => !e.Defeated);
            if (firstAlive < 0) return CloneWith(state, powerUpCharge: charge);
            targetIdx = firstAlive;
        }
        var enemies = state.Enemies.Select((e, i) => i != targetIdx || e.Defeated ? e : DamageEnemy(e, damage)).ToList();
        return CloneWith(state, enemies: enemies, targetIdx: targetIdx, powerUpCharge: charge);
    }

    static ActiveEnemy DamageEnemy(ActiveEnemy e, int damage, int? freezeTurns = null, int? distractTurns = null, double? distractAmount = null, bool clearShields = false)
    {
        var hp = Math.Max(0, e.Hp - damage);
        return new()
        {
            Id = e.Id, DefId = e.DefId, Name = e.Name, Hp = hp, MaxHp = e.MaxHp, Armor = e.Armor,
            Accuracy = e.Accuracy, Precision = e.Precision,
            Shields = clearShields ? new() : e.Shields.Select(s => new ShieldLayer { Type = s.Type, TargetValue = s.TargetValue }).ToList(),
            Defeated = hp <= 0, FrozenTurns = freezeTurns ?? e.FrozenTurns,
            VulnerableTurns = e.VulnerableTurns,
            DistractedTurns = distractTurns ?? e.DistractedTurns,
            DistractAmount = distractAmount ?? e.DistractAmount
        };
    }

    static CoopPlayer ClonePlayer(CoopPlayer p, int? powerUpCharge = null, List<PlayerBuff>? buffs = null) => new()
    {
        Id = p.Id, Name = p.Name, Color = p.Color, Hp = p.Hp, MaxHp = p.MaxHp, Power = p.Power, Armor = p.Armor,
        Buffs = buffs ?? p.Buffs, PowerUpCharge = powerUpCharge ?? p.PowerUpCharge, ClassId = p.ClassId,
        Kills = p.Kills, DamageDealt = p.DamageDealt
    };

    static ActiveEnemy CloneEnemy(ActiveEnemy e, int? frozenTurns = null, int? vulnerableTurns = null, int? distractedTurns = null, double? distractAmount = null, List<ShieldLayer>? shields = null) => new()
    {
        Id = e.Id, DefId = e.DefId, Name = e.Name, Hp = e.Hp, MaxHp = e.MaxHp, Armor = e.Armor,
        Accuracy = e.Accuracy, Precision = e.Precision,
        Shields = shields ?? e.Shields.Select(s => new ShieldLayer { Type = s.Type, TargetValue = s.TargetValue }).ToList(),
        Defeated = e.Defeated, FrozenTurns = frozenTurns ?? e.FrozenTurns,
        VulnerableTurns = vulnerableTurns ?? e.VulnerableTurns,
        DistractedTurns = distractedTurns ?? e.DistractedTurns,
        DistractAmount = distractAmount ?? e.DistractAmount
    };

    static CampaignBattleState CloneWith(CampaignBattleState s,
        List<CoopPlayer>? players = null, List<ActiveEnemy>? enemies = null, int? targetIdx = null,
        CoopOutcome? outcome = null, int? powerUpCharge = null, int? partyHp = null,
        int? phantomDarts = null, BattleStats? stats = null) => new()
    {
        LevelId = s.LevelId, LevelName = s.LevelName, IsBoss = s.IsBoss,
        PartyHp = partyHp ?? s.PartyHp, PartyMaxHp = s.PartyMaxHp,
        Players = players ?? s.Players, ChapterId = s.ChapterId,
        Stats = stats ?? s.Stats, PlayerTurnIdx = s.PlayerTurnIdx,
        Darts = s.Darts, Enemies = enemies ?? s.Enemies,
        TargetIdx = targetIdx ?? s.TargetIdx, Phase = s.Phase,
        VisitNumber = s.VisitNumber, Outcome = outcome ?? s.Outcome,
        PowerUpCharge = powerUpCharge ?? s.PowerUpCharge,
        ResolvedDarts = s.ResolvedDarts, VisitEnemiesSnapshot = s.VisitEnemiesSnapshot,
        PendingEnemyAttacks = s.PendingEnemyAttacks, AppliedEnemyAttacks = s.AppliedEnemyAttacks,
        AwaitContinue = s.AwaitContinue, PhantomDarts = phantomDarts ?? s.PhantomDarts,
        FrozenEnemiesThisRound = s.FrozenEnemiesThisRound, PassiveBonus = s.PassiveBonus
    };
}
