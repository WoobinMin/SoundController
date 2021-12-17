# SoundController

## 설명
유니티 인게임상에서 사운드를 쉽게 컨트롤 할 수 있는 Singleton script

## 세팅방법
1. Dotween이 필수적으로 요구됨(에셋스토어에 무료로 Import가능)
2. 올라와있는 Package를 Import

## 사용 방법 (Inspector)
### Prefab Setting
1. 자주사용하는 사운드의 경우 Common_clips에 추가 후 프리팹 저장
2. 각 씬마다 다르게 사용하는 경우 Act_Clips에 추가
3. 각 사운드의 네이밍은 아래와 같은 규칙으로 사용 (Name 부분에 원하는 이름 삽입)
    > **BGM** : BGM으로 사용되면 해당 씬 입장 시 자동으로 재생됨
    > 
    > **Eff_Name** : 이펙트사운드로 동시에 여러 소리가 나올 수 있음
    > 
    > **Narr_Name** : 한 번에 한개의 Narr 사운드만 재생됨(이전의 것을 강제 종료)
4. 실행 시 Common_Clips에 있는 Clip들이 Act_Clips으로 넘어옴

### Script
1. ``` Using DFP;```를 추가
2. ``` SoundController.instance.SoundControll("사운드이름") ```을 통해 사운드 재생
3. ``` SoundController.instance.GetDuration("사운드이름") ```을 통해 사운드 재생 길이를 return
4. ``` SoundController.instance.FindClip("사운드이름") ```을 통해 Act_Clips에 있는 Clip을 return

## 주의 사항
Common_Clips과 Act_Clips의 사운드 명이 겹칠 경우 Act_Clips의 사운드가 우선적으로 실행됨
