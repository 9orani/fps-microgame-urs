<div align="center">
	<img src="https://user-images.githubusercontent.com/29942514/195076609-ca0bea8a-bc16-45ed-9dca-d55cb34f5479.png" alt="fps-microgame-urs" width="450"></a>
</div>

<hr>

<div align="center">
  <h1>fps-microgame-urs</h1>
  <img src="https://img.shields.io/badge/Unity-2021.3.4f1-gray?logo=Unity&logoColor=black&labelColor=FFFFFF&style=flat-square"/>
  <br/>
  <img src="https://img.shields.io/badge/Unity_Render_Streaming-3.1.0_exp.3-gray?labelColor=000000&style=flat-square"/>
  <br/>
  <img src="https://img.shields.io/badge/Input_System-1.3.0-gray?labelColor=161A36&style=flat-square"/>
</div>

- 이 Repository는 [Unity fps-microgame](https://learn.unity.com/project/fps-maikeurogeim)와 [Eggcation-URS](https://github.com/9orani/unity_server)의 성능 비교를 위해 만들어 졌습니다.
- 상단의 Unity 교육용 프로젝트에 [Unity Render Streaming](https://docs.unity3d.com/Packages/com.unity.renderstreaming@3.1/manual/index.html)를 적용했습니다.
- 이에 Input System이 새롭게 교체되어 하단과 같이 수동으로 프로젝트 코드를 바꾸어 주었습니다.

## Migrate to new input system

- [https://docs.unity3d.com/Packages/com.unity.inputsystem@0.9/manual/Migration.html](https://docs.unity3d.com/Packages/com.unity.inputsystem@0.9/manual/Migration.html)
- 기본적으로 위 가이드 다수 참조


### Summary

- `Input.GetButtonDown` `Input.GetKeyDown` → `ButtonControl.wasPressedThisFrame`
- `Input.GetButtonUp` `Input.GetKeyUp` → `ButtonControl.wasReleasedThisFrame`
- `Input.GetButton` `Input.GetKey` → `ButtonControl.isPressed`

- (KeyBoard)`Input.GetAxisRaw` → `KeyControl.isPressed`
- (Mouse X)`Input.GetAxisRaw` → `Mouse.current.delta.x.ReadUnprocessedValue() / 20`
- (Mouse Y)`Input.GetAxisRaw` → `Mouse.current.delta.y.ReadUnprocessedValue() / -20`


### 가이드에 맞춰 변경한 경우

- `Input.GetButtonDown` `Input.GetKeyDown`
    
    ```csharp
    Input.GetButtonDown(MAPPED_KEY_STRING); // OLD
    Input.GetKeyDown(KEY_STRING); // OLD
    
    ((KeyControl)Keyboard.current[KEY_STRING]).wasPressedThisFrame; // NEW
    // KeyControl이 ButtonControl을 상속받음
    
    // other examples
    Keyboard.current.spaceKey.wasPressedThisFrame; // by Keyboard property
    Keyboard.current[Key.Space].wasPressedThisFrame; // by Key Enum
    ((KeyControl)Keyboard.current["space"]).wasPressedThisFrame; // by string
    ```
    
    - Key가 눌러진 순간 true
    - [Keyboard properties](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.Keyboard.html)
    - [Key Enum](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.Key.html) : returns integer

- `Input.GetButtonUP` `Input.GetKeyUP`
    
    ```csharp
    Input.GetButtonUp(MAPPED_KEY_STRING); // OLD
    Input.GetKeyUp(KEY_STRING); // OLD
    
    ((KeyControl)Keyboard.current[KEY_STRING]).wasReleasedThisFrame; // NEW
    ```
    
    - Key가 떼어질 때 true
    
- `Input.GetButton` `Input.GetKey`
    
    ```csharp
    Input.GetButton("Fire"); // OLD
    Input.GetKey(KeyCode.Mouse0); // OLD
    
    Mouse.current.leftButton.isPressed; // NEW
    ```
    
    - Key가 눌러진 상태면 true
    - [KeyCode Enum](https://docs.unity3d.com/ScriptReference/KeyCode.html) : returns integer


### 가이드의 도움을 받지 못한 경우

- Not directly applicable. You can use access any `InputControl<>.ReadUnprocessedValue()` to read unprocessed values from any control.
- 위와 같은 식으로 가이드도 1대1 치환을 못하는 경우가 있었다.

- `Input.GetAxisRaw`
    - Keyboard 입력의 경우
        
        ```csharp
        // Case 1
        Input.GetAxisRaw("Vertical") != 0; // OLD
        
        Keyboard.current.wKey.isPressed
        	|| Keyboard.current.sKey.isPressed; // NEW
        
        // Case 2
        Input.GetAxisRaw("Horizental"); // OLD
        
        getAxisRawKeyboard("Horizental"); // NEW
        ```
        
        - 키보드 입력 상에서의 `Input.GetAxisRaw` 는 WASD 입력에 따라 1.0, 0.0, -1.0을 반환하는 단순한 동작을 한다.
        - 이에 간단한 조건절 이나, 해당 값들을 반환하는 간단한 함수(`getAxisRawKeyboard` )를 만들어준다.
        - `getAxisRawKeyboard`
            
            ```csharp
            protected float getAxisRawKeyboard(string axis){
                    if(axis == GameConstants.k_AxisNameHorizontal){
                        return getAxisRawHorizental();
                    }
                    else if(axis == GameConstants.k_AxisNameVertical){
                        return getAxisRawVertical();
                    }
                    else{
                        return 0.0f;
                    }
                }
            protected float getAxisRawHorizental(){
                if(Keyboard.current.dKey.isPressed){
                    return 1.0f;
                }
                else if(Keyboard.current.aKey.isPressed){
                    return -1.0f;
                }
                else{
                    return 0.0f;
                }
            }
            protected float getAxisRawVertical(){
                if(Keyboard.current.wKey.isPressed){
                    return 1.0f;
                }
                else if(Keyboard.current.sKey.isPressed){
                    return -1.0f;
                }
                else{
                    return 0.0f;
                }
            }
            ```
            
    
    - Mouse 입력의 경우
        
        ```csharp
        float i = Input.GetAxisRaw(MOUSE_AXIS); // OLD
        
        float i = 0.0f; // NEW
        if(MOUSE_AXIS == "Mouse X"){
            i = Mouse.current.delta.x.ReadUnprocessedValue() / 20;
        }
        else if(MOUSE_AXIS == "Mouse Y"){
            i = Mouse.current.delta.y.ReadUnprocessedValue() / -20;
        }
        else{
            i = 0.0f;
        }
        ```
        
        - MOUSE_AXIS : “Mouse X” 혹은 “Mouse Y”입니다.
        - 가이드에 언급된 `ReadUnprocessedValue` 값과 `Input.GetAxisRaw` 값이 20배 차이가 나는 것을 콘솔 출력으로 확인하여(수직방향의 경우 -20)  대체했습니다.


## 결과

<img style="width:50%" src="https://user-images.githubusercontent.com/29942514/195080372-284bb065-fd0a-4e45-a3f5-c03c1c796d7b.png"/>

- 웹에서의 동작을 성공적으로 확인

### 성능 측정

<img style="width:50%" src="https://user-images.githubusercontent.com/29942514/195080537-1f277770-50af-464b-9c80-d1a9ae10ecfe.png"/>

- Eggcation-URS와 비교한 bitrate 값 측정
- 컴퓨터 환경: MacOS 12.5, Chrome 105.0.5195.127
- 네트워크 환경: 외부 네트워크– 47Mbps
