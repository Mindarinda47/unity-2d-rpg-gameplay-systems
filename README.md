# Unity 2D RPG Gameplay Systems

Unity 2D RPG에서 플레이어 상호작용을 중심으로 퀘스트, 인벤토리, 전투, 대화, 저장 시스템을 연결한 개인 프로젝트입니다.

게임플레이 프로그래밍 포트폴리오를 위해 주요 소스 코드와 설계 문서를 공개하며 외부 아트·폰트 리소스는 포함하지 않습니다.

## 3분 검토 경로

1. [QuestManager](Assets/Scripts/Manager/QuestManager.cs)와 [QuestData](Assets/Scripts/Quest/QuestData.cs)에서 퀘스트 상태와 진행도 관리를 확인합니다.
2. [PlayerInventory](Assets/Scripts/Inventory/PlayerInventory.cs), [SaveManager](Assets/Scripts/Manager/SaveManager.cs)에서 런타임 데이터와 저장 데이터의 연결을 확인합니다.
3. [EnemyAI](Assets/Scripts/Enemy/EnemyAI.cs), [PlayerCombat](Assets/Scripts/Player/PlayerCombat.cs), [EnemyHealth](Assets/Scripts/Enemy/EnemyHealth.cs)에서 전투 상태 전환과 피격 흐름을 확인합니다.
4. 전체 시스템 관계는 [ARCHITECTURE.md](Docs/ARCHITECTURE.md), 주요 개선 사례는 [PROBLEM_SOLVING.md](Docs/PROBLEM_SOLVING.md)에서 확인할 수 있습니다.

## 프로젝트 목적

탐색, NPC 대화, 퀘스트 수행, 전투, 보상, 저장·불러오기가 하나의 플레이 흐름으로 이어지는 2D RPG를 구현하는 것이 목표였습니다. 개별 기능보다 시스템 사이의 상태 전달과 UI 동기화에 중점을 두었습니다.

## 담당 역할 및 구현 범위

개인 프로젝트로 게임플레이 로직과 시스템 연동을 구현했습니다.

- 플레이어 이동, 상호작용, 공격, 체력, 아이템 사용
- 적 추적·공격 상태 전환, 피격, 넉백, 사망 처리
- 퀘스트 상태·진행도·보상과 NPC 대화 분기 연동
- 수량 기반 인벤토리, 아이템 획득·소비, 소지금 관리
- 소지금·인벤토리·퀘스트 상태의 JSON 저장 및 복원
- 이벤트 기반 HUD·인벤토리·퀘스트 UI 갱신

## 기술 스택

- Unity 6 (`6000.3.14f1`)
- C# / MonoBehaviour / ScriptableObject
- Unity UI / TextMesh Pro
- JSON 파일 저장
- 2D Physics 기반 상호작용·전투 판정

## 핵심 시스템과 대표 코드

| 시스템 | 구현 내용 | 대표 코드 |
|---|---|---|
| 퀘스트 | 상태 전이, 목표 진행도, 추적 퀘스트, 보상 처리 | [QuestManager](Assets/Scripts/Manager/QuestManager.cs), [QuestData](Assets/Scripts/Quest/QuestData.cs) |
| 인벤토리 | 동일 아이템 수량 병합, 추가·제거·조회, 변경 이벤트 | [PlayerInventory](Assets/Scripts/Inventory/PlayerInventory.cs), [InventoryEntry](Assets/Scripts/Inventory/InventoryEntry.cs) |
| 저장 | 소지금·인벤토리·퀘스트 상태의 JSON 저장 및 복원 | [SaveManager](Assets/Scripts/Manager/SaveManager.cs), [GameSaveData](Assets/Scripts/SaveData/GameSaveData.cs) |
| 적 행동 | 거리 기반 추적·공격 전환, 공격 범위 히스테리시스, 넉백 연동 | [EnemyAI](Assets/Scripts/Enemy/EnemyAI.cs), [EnemyHealth](Assets/Scripts/Enemy/EnemyHealth.cs) |
| 플레이어 전투 | 공격 쿨다운, 범위 판정, 피해 전달, 공격 애니메이션 | [PlayerCombat](Assets/Scripts/Player/PlayerCombat.cs), [IDamageable](Assets/Scripts/Interaction/IDamageable.cs) |
| 대화·상호작용 | NPC별 대화 진행, 퀘스트 상태 분기, 선택지와 퀘스트 동작 연결 | [NPCInteractable](Assets/Scripts/Interaction/NPCInteractable.cs), [DialogueManager](Assets/Scripts/Manager/DialogueManager.cs) |

## 설계 의도

- 퀘스트 데이터가 상태 전이 규칙을 소유하고, `QuestManager`가 검색·진행·보상과 이벤트 발행을 담당하도록 역할을 분리했습니다.
- 인벤토리 내부 컬렉션은 읽기 전용으로 노출하고 추가·제거 경로를 한곳으로 모아 수량 불일치를 줄였습니다.
- 전투 대상은 `IDamageable` 계약으로 연결해 공격 주체가 적 체력 구현에 직접 의존하지 않도록 구성했습니다.
- UI는 퀘스트·인벤토리·전투 이벤트를 구독해 게임 상태 변경과 표시 갱신을 분리했습니다.
- 저장 데이터는 Unity 오브젝트 참조 대신 ID와 값 중심의 구조로 변환해 복원 경계를 명확히 했습니다.

## 문제 해결 사례

- 적이 공격 경계에서 추적과 공격을 반복하던 현상을 서로 다른 진입·이탈 거리로 분리해 상태 전환을 안정화했습니다.
- 저장 데이터를 불러온 뒤 퀘스트 내부 값만 복원되고 UI가 이전 표시를 유지하던 문제를 상태·진행도 이벤트 재발행으로 해결했습니다.
- 손상된 JSON, 누락된 목록, 존재하지 않는 아이템·퀘스트 ID가 있어도 전체 불러오기가 즉시 중단되지 않도록 입력 경계를 방어했습니다.

구현 배경과 검증 기준은 [PROBLEM_SOLVING.md](Docs/PROBLEM_SOLVING.md)에 정리했습니다.

## 실행 및 시연

이 저장소는 주요 C# 구현과 설계 설명을 검토하기 위한 코드 포트폴리오입니다. 외부 아트·폰트와 실행 프로젝트 전체는 포함하지 않습니다.

시연 영상은 지원서 제출 자료에 별도로 포함합니다.

## 저장소 구성

```text
Assets/Scripts/          게임플레이 C# 소스
Docs/ARCHITECTURE.md     시스템 구성과 데이터 흐름
Docs/PROBLEM_SOLVING.md  문제 상황, 판단, 해결 및 검증
```
