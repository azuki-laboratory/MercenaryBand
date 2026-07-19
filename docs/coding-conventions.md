# 코딩 규칙

## 명명 규칙

- 네임스페이스: PascalCase (`MercenaryBand.Combat`)
- 클래스/구조체: PascalCase (`HexGrid`)
- 메서드: PascalCase (`GetStat`)
- 프로퍼티: PascalCase (`CurrentMode`)
- private 필드: _camelCase (`_tiles`)
- 로컬 변수: camelCase (`unitCount`)
- 상수: PascalCase (`MaxLevel`)
- Enum: PascalCase (`StatModType.Add`)

## 파일 구성

- 하나의 파일에 하나의 주요 클래스
- 열거형은 별도 파일로 분리하거나 관련 클래스와 같은 파일에 배치
- using 문: System → Godot → 프로젝트 내부 순서

## C# 스타일

```csharp
// ✅ 좋은 예
public float GetStat(string statName)
{
    float baseValue = _baseStats.GetValueOrDefault(statName, 0f);
    return ApplyModifiers(baseValue, statName);
}

// ❌ 나쁜 예
public float get_stat(string sn) { ... }
```

## Godot 통합

- `[GlobalClass]`는 필요한 경우에만 사용 (에디터 노출이 필요한 클래스)
- `_Ready()`는 Godot 라이프사이클 진입점
- `GD.Print()` / `GD.PushWarning()` / `GD.PushError()`로 로깅
- 씬 간 통신은 Signal 또는 Autoload를 통해

## JSON 데이터

- 모든 Def 클래스는 `scripts/Data/`에 배치
- JSON 키 = C# 프로퍼티명 (PascalCase)
- Enum은 문자열로 (`"Add"`, `"Multiply"`)
- `Id` 필드는 항상 소문자 + 언더스코어 (`"farmhand"`, `"shortsword"`)

## 오류 처리

- JSON 파일 누락 → `GD.PushWarning()` + null 반환
- 필수 데이터 누락 → `GD.PushError()` + 기본값 fallback
- 잘못된 입력 → 방어적 코딩 (null 체크, 기본값)
