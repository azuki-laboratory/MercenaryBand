# 아키텍처 개요

## 게임 모드 상태 머신

```
MainMenu → Overworld ↔ Combat
                ↕          ↕
              Town        Camp
```

GameManager.CurrentMode로 상태 전환.

## 데이터 흐름

```
data/*.json → DataLoader (캐싱) → CharacterFactory / 기타 시스템
                 ↓
          CharacterSheet (런타임) → CombatUnit (전투 인스턴스)
```

## 스탯 계산 파이프라인

```
BaseStats (배경) → + LevelUps → + Traits → + Perks → + Equipment → + Injuries
                                                                    ↓
                                            최종 스탯 = (Base + ∑Add) * ∏Multiply
```

## 전투 시스템 구조

```
CombatScene
├── HexGrid
│   └── HexTile[*]         ← 타일 데이터 (지형, 점유 유닛)
├── CombatUnit[*]          ← 전투 중인 모든 유닛
├── TurnOrder              ← 우선순위 큐 (Initiative 기반)
├── CombatActions          ← 가능한 행동 정의
└── CombatCalculator       ← 명중/대미지/사기 판정
```

## Hex 좌표계

- 축 좌표계 (Axial): Q, R
- 평평한 윗면 (Flat-top) 헥스
- 6방향: NE(1,-1), E(1,0), SE(0,1), SW(-1,1), W(-1,0), NW(0,-1)

## 캐릭터 시스템

```
BackgroundDef (JSON)
    ↓
CharacterFactory.Create(backgroundId, level)
    ↓
CharacterSheet (런타임)
    ├── GetStat(statName) → 최종 계산값
    ├── Equip(itemId) → 장비 장착
    ├── AddPerk(perkId) → 퍽 추가
    └── LevelUp() → 레벨 증가 + 퍽 선택
```
