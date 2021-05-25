#### Morai rpc with google protobuf

 [gRPC](https://morai.atlassian.net/wiki/spaces/SWDEV/pages/465469445/gRPC+Google+Remote+Procedure+Calls)

- protobuf를 타겟별로 정의하고 빌드 및 테스트 할 수 있습니다. 

- 추가적인 protobuf가 생성될 수 있다는 가정하에 정리되어 있습니다.

#### Requirements
1. IDE
- visual studio 2019
2. Nuget
- Google.Protobuf v4.0.30319
- Google.Protobuf.Tools v3.15.1
- Grpc.Core v4.0.30319
- Grpc.Core.Api v4.0.30319
3. Unity 2020.2.0a18

#### VS solution

![image](/uploads/1cc5c3cd143506c825312ecc5f84d1af/image.png)

* protobuf 는 타겟별로 생성하여야 합니다. 
* 생성된 Protobuf 프로젝트를 ServerTest & ClientTest가 참조하여 사용합니다. 

#### Architecture

![image](/uploads/9507cbf2774af6afd8c0f9d0969c753a/image.png)

- SSP : simulator support package
- DSP - DUT support package
- protobuf (version) :  protocol buffers are Google's language-neutral, platform-neutral, extensible mechanism for serializing structured data

#### Class diagram

- Composite 패턴

![image](/uploads/54a3a32d30f808cbebdbd449a45e42cd/image.png)

#### ServerTest & ClientTest
 정의된 protobuf를 참조하여 Message와 Service가 동작하는지 여부를 테스트 할 수 있습니다. 

#### Note
- proto file을 정의 하고 빌드하게 되면 아래 그림과 같이 *.cs 파일이 생성 됨
![image](/uploads/046bf93e08df96c5c193c9c414b4469f/image.png)

- 해당 파일을 Unity Asset 폴더에 카피하여 사용하는 형태

- 빌드한 후에 소스를 자동으로 카피 해주기 위해서 빌드 후 이벤트 사용

```
// create folder
if not exist "$(SolutionDir)UnityTest\Assets\Scripts\Foretify" mkdir "$(SolutionDir)UnityTest\Assets\Scripts\Foretify"
if not exist "$(SolutionDir)UnityTest\Assets\Scripts\Foretify\Protobuf" mkdir "$(SolutionDir)UnityTest\Assets\Scripts\Foretify\Protobuf"

// copy file
copy  /y "$(SolutionDir)\ProtobufForetify\obj\debug\protos\Foretify.cs" "$(SolutionDir)UnityTest\Assets\Scripts\Foretify\Protobuf\Foretify.cs"
copy  /y "$(SolutionDir)\ProtobufForetify\obj\debug\protos\ForetifyGrpc.cs" "$(SolutionDir)UnityTest\Assets\Scripts\Foretify\Protobuf\ForetifyGrpc.cs"
```

![image](/uploads/5cce5fe59094c6dbf4c202204b258278/image.png)

#### Auto copy files ForetifyLinker *.cs 
```
if not exist "$(SolutionDir)UnityTest\Assets\Scripts\Foretify" mkdir "$(SolutionDir)UnityTest\Assets\Scripts\Foretify"
if not exist "$(SolutionDir)UnityTest\Assets\Scripts\Foretify\Linker" mkdir "$(SolutionDir)UnityTest\Assets\Scripts\Foretify\Linker"
xcopy "$(SolutionDir)ForetifyLinker\ForetifyLinker\*.*" "$(SolutionDir)UnityTest\Assets\Scripts\Foretify\Linker" /Y /I /E
```

![image](/uploads/4d52635f4d9366957a551002cef0dafa/image.png)


#### Header 구조

```
from struct import *

header = pack('!HbIb', 1, 0, 41, 1)
# ! big endian
# H unsigned short      2           [0][1]              # rpc_id ex) init_req = 1, launch_sim = 2 ...
# b signed char         1           [0]2                # direction : REQUEST = 0, REPLY = 1
# I Long                4           [0][0][0][41]       # data size
# b signed char         1           [1]                 # rpc_result : Fail = 0, Success = 1

```

* rpc_id
```
public enum SSP_MSG_ID
{ 
	None = 0,
	init = 1,
	launch_sim = 2,
	terminate_sim = 3,
	start_sim = 4,
	wait_start_sim = 5,
	end_sim = 6, 
	start_step = 7, 
	wait_step = 8,
	get_actor_state = 9,
	create_actor = 10,
	set_actor_params = 11,
	destroy_actor = 12,
	set_weather = 13,
	set_time_of_day = 14,
	sim_command = 15,
	set_xy_trajectory_move = 16,
	set_steer_and_pedals_move = 17,
	set_external_controller_move = 18
}

public enum DSP_MSG_ID
{
	None = 0,
	init = 1,
	start_sim = 2,
	wait_start_sim = 3,
	end_sim = 4,
	start_step = 5,
	wait_step = 6,
	set_ego_control = 7,
	get_ego_info = 8,
	ego_command = 9,
	set_ego_destination = 10,
	set_steer_and_pedals_move = 11,
	send_messages = 12,
	get_messages = 13, 
}
```

#### How to convert protobuf type class from packet data (byte[])

* MessageParser 이용

https://developers.google.com/protocol-buffers/docs/reference/csharp/class/google/protobuf/message-parser

#### Example

##### [Receiver 클래스 만드는 방법]
- IReceiver를 상속받아 메세지 수신시에 콜을 발생 시킬 수 있습니다. 
- 수신된 SSP_MSG_ID 에 따라서 각각 다른 data가 전송되며 Converter 클래스를 통해서 변환할 수 있습니다.
- Response 프로퍼티를 이용하여 클라이언트에 Response를 보낼 수 있습니다.

```
using System;
using Morai.Protobuf.Foretify;
using ForetifyLinker;

class Receiver : IReceiver
{
	public IResponse Response { get; set; }

	public void Receive(SSP_MSG_ID id, byte[] arr)
	{
		Console.WriteLine("----------------------------------");
		Console.WriteLine($"[Request msg id : {(int)id}.{id}]");

		if (id == SSP_MSG_ID.init)
		{
			// request
			init_req req = Converter.ToObject<init_req>(arr);
			
			Console.WriteLine($"step size : {req.Info.StepSizeMs}");
			Console.WriteLine($"map info : {req.Info.MapInfo}");

			// return
			init_resp resp = new init_resp
			{ 
				Status = new status
				{ 
					Info = { "init_resp", "ok" },
				}
			};
			Console.WriteLine("-> response msg : init_resp");
			Response.SendData(id, resp);
		}
	}
}
```

##### [main class]
- Receiver 클래스의 인스턴스를 Server 생성자로 전달해줘야 합니다. 
- StatusEvent의 경우 각종 서버의 상태를 리턴 받을 수 있습니다. 
- Foretify 에뮬레이터의 경우 7788포트를 사용합니다. 

``` 
using System;
using System.Threading.Tasks;
using ForetifyLinker;

class Program
{
	public static void Main(string[] args)
	{
		// how to use
		IServer manager = new Server();
		manager.AddReceiver(new Receiver());
		manager.StatusEvent += ServerStatus;
		manager.Start("127.0.0.1", "7788");
		//manager.Start(Define.URL, Define.Port);

		Console.ReadKey();
	}
	
	public static void ServerStatus(string msg)
	{
		// event message
		Console.WriteLine(msg);
	}
}
```

#### etc
[Execution_Platform_Integration..pdf](/uploads/9d39146b5efb8c016393d4d8c329cc02/Execution_Platform_Integration..pdf)

[How to make gRPC porject in visual studio](https://morai.atlassian.net/wiki/spaces/SWDEV/pages/476447114/How+to+make+gRPC+project+in+visual+studio.)

[다이어그램 문서 위치](https://app.diagrams.net/#G1PGr1REielWEIsKjcjOgXxAOUpNVX4i_9)

#### screen shot

![image](/uploads/80aa0fd1f89c630fe947cb7f7a2446a9/image.png)
