# MercenaryBand — AI Agent Development Guide

## 프로젝트 개요

장르: 2D 파티 기반 RPG 샌드박스 시뮬레이션 (Battle Brothers 컨셉)
엔진: Godot 4.7.1 .NET Edition (Mono)
언어: C# (.NET 9.0)
IDE: Google Anti-Gravity (C# 편집 전용), Godot Editor (씬 편집/테스트)

## 필수 환경

- Godot Engine (Mono) 4.7.1 — `winget install GodotEngine.GodotEngine.Mono`
- .NET SDK 9.0 — `winget install Microsoft.DotNet.SDK.9`
- Git

## 실행 방법

```powershell
godot --path "C:\development\MercenaryBand" --editor
godot --headless --path "C:\development\MercenaryBand" --quit   # CLI 테스트
dotnet build .\MercenaryBand.csproj                               # C#만 빌드
```

## 프로젝트 구조

```
MercenaryBand/
├── assets/             ← 원시 에셋 (스프라이트, 사운드, 폰트, 셰이더)
├── data/               ← JSON 게임 데이터 (데이터 주도 설계)
├── docs/               ← AI 에이전트용 문서
├── scenes/             ← Godot .tscn 씬 파일
│   ├── autoloads/      ← 싱글톤 씬
│   ├── combat/         ← 전투 관련 씬
│   ├── overworld/      ← 월드맵
│   ├── ui/             ← UI 씬
│   └── management/     ← 야영/마을
├── scripts/            ← C# 소스
│   ├── Core/           ← GameManager, DataLoader, SaveManager
│   ├── Combat/         ← 전투 시스템
│   ├── Data/           ← 데이터 모델 (JSON 매핑)
│   ├── Overworld/      ← 월드맵
│   ├── Systems/        ← 경제, 평판, 이벤트, 캐릭터 팩토리
│   └── UI/             ← UI 컨트롤러
└── addons/             ← 서드파티 플러그인
```

## 핵심 설계 원칙

### 1. 데이터 주도 설계 (Data-Driven)
모든 정적 게임 데이터는 `data/` 폴더의 JSON 파일로 관리한다.
새로운 퍽/아이템/특성 추가는 JSON만 수정하면 되고, C# 코드 변경이 필요 없다.

### 2. StatModifier 시스템
모든 스탯 변경(퍽, 특성, 부상, 장비)은 `StatModifier`로 통일한다:
- `Add`: 고정값 증감
- `Multiply`: 비율 증감 (1.25 = 25% 증가)

### 3. 컴포지션 > 상속
퍽, 특성 등은 상속 계층이 아닌 데이터 정의로 관리한다.
`class PerkBerserker : BasePerk` ❌
`PerkDef { Id="berserk", Modifiers=[...] }` ✅

### 4. 네임스페이스 규칙
```
MercenaryBand.Core      — GameManager, DataLoader, SaveManager
MercenaryBand.Data      — 데이터 모델 정의 (Def 클래스, CharacterData)
MercenaryBand.Combat    — 전투 시스템
MercenaryBand.Overworld — 월드맵
MercenaryBand.Systems   — 게임플레이 시스템
MercenaryBand.Scripts   — 루트 레벨 스크립트 (Main.cs)
```

### 5. JSON 직렬화
- `System.Text.Json` 사용 (NuGet 불필요)
- Enum 값은 문자열로 저장 (`[JsonConverter(typeof(JsonStringEnumConverter))]`)
- C# 프로퍼티 이름과 JSON 키는 PascalCase로 일치시킨다

## Autoload (싱글톤)

| 이름 | 타입 | 역할 |
|------|------|------|
| `DataManager` | DataLoader | JSON 데이터 로드/캐싱/조회 |
| `GameState` | GameManager | 게임 모드 전환 (MainMenu/Overworld/Combat/Town/Camp) |
| `SaveManager` | SaveManager | 세이브/로드 관리 |

접근: `GetNode<DataLoader>("/root/DataManager")`

## 테스트 방법

```powershell
dotnet build .\MercenaryBand.csproj    # C# 컴파일 체크
godot --headless --path . --quit       # 게임 실행 + 콘솔 출력 확인
```

## 변경 시 주의사항

1. `data/` JSON 변경 → `DataLoader`가 자동 반영
2. `scripts/Data/` 모델 클래스 변경 → JSON 구조와 일치하는지 확인
3. Godot `.tscn` 파일은 수동 편집보다 Godot Editor에서 생성 권장
4. `[Tool]` 어트리뷰트가 붙은 스크립트는 에디터에서도 실행되므로 주의
5. `project.godot` 수정 시 `config/features`에 `"C#"`이 반드시 포함되어야 함
