# 데이터 모델 참조

## StatModifier 시스템

모든 스탯 변경은 `StatModifier`로 표현된다:

```json
{
    "Stat": "Hitpoints",
    "Type": "Add",        // Add: 고정값, Multiply: 배율
    "Value": 15
}
```

최종 스탯 = (Base + sum(Add)) * product(Multiply)

## 캐릭터 속성 목록

| Stat Name | 설명 | 기본 용도 |
|-----------|------|-----------|
| Hitpoints | 체력 | 0 이하 → 사망 |
| Fatigue | 피로도 | 행동 코스트, 방어구 페널티 |
| Resolve | 의지 | 사기 판정 |
| Initiative | 우선도 | 턴 순서 결정 |
| MeleeSkill | 근접 공격력 | 명중률 계산 |
| RangedSkill | 원거리 공격력 | 명중률 계산 |
| MeleeDefense | 근접 방어력 | 회피율 계산 |
| RangedDefense | 원거리 방어력 | 회피율 계산 |
| ExperienceGain | 경험치 획득 배율 | (Multiply 전용) |
| ActionPoints | 행동력 | 턴당 이동/공격 가능 횟수 |

## JSON 파일 스키마

### backgrounds.json
```jsonc
{
    "Id": "farmhand",
    "Name": "농장 일꾼",
    "Description": "...",
    "BaseStats": { "StatName": { "Min": 0, "Max": 0 } },
    "ExcludedTraits": ["id1"],
    "WeightedTraits": [{ "TraitId": "id", "Weight": 3 }],
    "HiringCost": { "Min": 0, "Max": 0 },
    "DailyWage": 0
}
```

### perks.json
```jsonc
{
    "Id": "berserk",
    "Name": "광전사",
    "Tier": 3,
    "Description": "...",
    "Modifiers": [{ "Stat": "...", "Type": "Add", "Value": 4 }],
    "Requirements": {
        "MinLevel": 7,
        "RequiredStats": { "Fatigue": 120 },
        "RequiredPerks": ["student"]
    }
}
```

### weapons.json
```jsonc
{
    "Id": "shortsword",
    "Name": "단검",
    "Type": "OneHanded",   // OneHanded, TwoHanded
    "DamageMin": 20,
    "DamageMax": 35,
    "ArmorDamage": 60,      // 방어구 파괴 %
    "ArmorPenetration": 15, // 방어구 무시 %
    "FatiguePerSwing": 10,
    "Skills": ["stab"],
    "Value": 100,
    "Durability": 72,
    "MaxFatiguePenalty": -2,
    "TwoHanded": false
}
```

### armors.json
```jsonc
{
    "Id": "chainmail",
    "Name": "사슬 갑옷",
    "Slot": "Body",         // Head, Body
    "Armor": 80,
    "Durability": 100,
    "MaxFatiguePenalty": -8,
    "Value": 300
}
```

### XP 곡선 (ExperienceTable.cs)

| Level | XP 필요 |
|-------|---------|
| 1 | 0 |
| 2 | 200 |
| 3 | 500 |
| 4 | 1,000 |
| 5 | 1,800 |
| 6 | 2,800 |
| 7 | 4,000 |
| 8 | 5,500 |
| 9 | 7,500 |
| 10 | 10,000 |
| 11 | 14,000 |
