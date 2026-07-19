using System;
using Godot;

namespace MercenaryBand.Combat;

public class HitResult
{
    public bool Hit { get; set; }
    public int Damage { get; set; }
    public int ArmorDamage { get; set; }
    public bool Killed { get; set; }
}

public class CombatCalculator
{
    private const int BaseHitChance = 50;

    public HitResult CalculateMeleeHit(CombatUnit attacker, CombatUnit defender)
    {
        int hitChance = BaseHitChance + attacker.MeleeSkill - defender.MeleeDefense;
        hitChance = Math.Clamp(hitChance, 5, 95);

        var rng = new Random();
        bool hit = rng.Next(100) < hitChance;

        if (!hit)
            return new HitResult { Hit = false };

        int damageMin = attacker.MainHandWeapon?.DamageMin ?? 5;
        int damageMax = attacker.MainHandWeapon?.DamageMax ?? 15;
        int weaponDamage = rng.Next(damageMin, damageMax + 1);

        int armorPen = attacker.MainHandWeapon?.ArmorPenetration ?? 10;

        int hpDamage = weaponDamage * armorPen / 100;
        int armorDmg = weaponDamage - hpDamage;

        if (defender.BodyArmor > 0)
        {
            int absorbed = Math.Min(armorDmg, defender.BodyArmor);
            defender.BodyArmor -= absorbed;
            defender.Hitpoints -= Math.Max(0, hpDamage - Math.Max(0, armorDmg - absorbed));
        }
        else
        {
            defender.Hitpoints -= weaponDamage;
        }

        bool killed = defender.Hitpoints <= 0;
        if (killed)
        {
            defender.Hitpoints = 0;
            GD.Print($"[Combat] {attacker.UnitName} killed {defender.UnitName}!");
        }
        else
        {
            GD.Print($"[Combat] {attacker.UnitName} hit {defender.UnitName} for {weaponDamage} damage (HP: {defender.Hitpoints})");
        }

        return new HitResult
        {
            Hit = true,
            Damage = weaponDamage,
            ArmorDamage = armorDmg,
            Killed = killed
        };
    }
}
