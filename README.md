# KeepMabinogiScreenShot
마비노기 영웅전을 즐기던 중 스크린 샷이 잘 나와서 저장을 하고 싶은데, 
방장이 파티를 너무 빨리 나가버려 저장 버튼을 누르지 못하는 참사를 번번히 겪었다.

마비노기 영웅전은 스크린샷 기본 폴더(내 경우엔 %USERPROFILE%\\Documents\\마비노기 영웅전\\스크린샷\\)에 "_mheroes_ss_"를 prefix로 가지는
임시파일이 일반 던전일 경우 6개 생성이 되고, 유저가 저장 버튼을 누르지 않으면 그 파일들이 삭제된다.

그러한 임시파일들을 보존하기 위해 이 프로그램을 만들었다.

이 프로그램은 
기본적으로 setup.json에 환경 설정을 읽어서 실행한다.
만약 내 PC와 환경이 다르다면, setup.json에 스크린샷 폴더 경로를 설정하면 된다.

이 프로그램은 3가지 버튼으로 구성되어 있다.
1. Run button
2. Force stop button
3. Move backup files button

1. Run button
    - Run 버튼을 누르면 이러한 임시 파일들이 6개가 생길때 까지 모니터링 하다가 생성이 되면 스크린샷 기본 폴더\backup으로 안전하게 일단 옮겨 놓는다.
    - 모니터링 상태라면 버튼이 빨간색으로 점멸한다.
    - backup 폴더로 다 옮기면 점멸이 끝난다.    
    - 클릭 하는 번거로움을 피하기 위해 enter키를 눌러도 run 버튼이 실행된다.

2. Force stop
    - Run button의 점멸 상태를 끄고 모니터링을 중지 시킨다.

3. Move Backup Files
    - Backup 시킨 파일은 jpg 형태이긴 한데, 확장자가 없는 임시 이름으로, 이 버튼을 누르면 스크린샷 폴더로 jpg 형태로 바꾸어 저장해준다.
    - Naming은 알아서 당일 저장된 스크린샷의 마지막 숫자에 이어서 만든다.

Build 환경
    Windows 10
    Visual studio 2019 community
    .Net Framework 4.7.2

Dependency - Newtonsoft.Json
    
