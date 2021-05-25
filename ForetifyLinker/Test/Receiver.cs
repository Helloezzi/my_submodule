using System;
using System.IO;
using System.Text;
using Morai.Protobuf.Foretify;

namespace ForetifyLinker
{
    class Receiver : IReceiver
    {
        public IResponse Response { get; set; }
        public IDebug xDebug { get; set; }

        public void Receive(SSP_MSG_ID id, byte[] arr)
        {
            xDebug.Write("----------------------------------");
            xDebug.Write($"[Request msg id : {(int)id}.{id}]");

            if (id == SSP_MSG_ID.init)
            {
                // request
                init_req req = Converter.ToObject<init_req>(arr);
                
                xDebug.Write($"step size : {req.Info.StepSizeMs}");
                xDebug.Write($"map info : {req.Info.MapInfo}");

                // create response message
                init_resp resp = new init_resp();
                resp.Status = new status
                {
                    Info = { "init_resp", "ok" },
                };

                xDebug.Write("-> response msg : init_resp");
                Response.SendData(id, resp);
            }
            else if (id == SSP_MSG_ID.launch_sim)
            {
                launch_simulator_req req = Converter.ToObject<launch_simulator_req>(arr);
                xDebug.Write("launch_simulator_req");
                xDebug.Write($"ConnectionString : {req.ConnectionString}");

                // create response message
                launch_simulator_resp resp = new launch_simulator_resp
                {
                    ConnectionString = "response test"
                };
                xDebug.Write("-> response msg : launch_simulator_resp");
                Response.SendData(id, resp);
            }
            else if (id == SSP_MSG_ID.start_sim)
            {
                start_simulation_req req = Converter.ToObject<start_simulation_req>(arr);

                xDebug.Write("start_simulation_req");
                xDebug.Write($"ConnectionString : {req.ConnectionString}");

                // create response message
                start_simulation_resp resp = new start_simulation_resp
                {
                    Status = new status
                    {
                        Info = { "start_simulation_resp" },
                    }
                };
                xDebug.Write("-> response msg : start_simulation_resp");
                Response.SendData(id, resp);
            }
            else if (id == SSP_MSG_ID.wait_start_sim)
            {
                wait_start_simulation_req req = Converter.ToObject<wait_start_simulation_req>(arr);
                xDebug.Write("wait_start_simulation_req");
                xDebug.Write($"MaxWaitMs : {req.MaxWaitMs}");

                // create response message
                wait_start_simulation_resp resp = new wait_start_simulation_resp
                { 
                    Status = new status
                    {
                        Info = { "wait_start_simulation_resp" }
                    }
                };
                xDebug.Write("-> response msg : wait_start_simulation_resp");
                Response.SendData(id, resp);
            }
            else if (id == SSP_MSG_ID.end_sim)
            {
                //end_simulation_req req = Converter.ToObject<end_simulation_req>(arr);
                //xDebug.Write("end_simulation_req");

                // create response message
                end_simulation_resp resp = new end_simulation_resp
                { 
                    Status = new status
                    { 
                        Info = { "end_simulation_resp" }
                    }
                };
                xDebug.Write("-> response msg : end_simulation_resp");
                Response.SendData(id, resp);
            }
            else if (id == SSP_MSG_ID.terminate_sim)
            {
                // create response message
                terminate_simulator_resp resp = new terminate_simulator_resp
                {
                    Status = new status
                    {
                        Info = { "terminate_simulator_resp" }
                    }
                };
                xDebug.Write("-> response msg : terminate_simulator_resp");
                Response.SendData(id, resp);
            }
            else if (id == SSP_MSG_ID.create_actor)
            {
                create_actor_req req = Converter.ToObject<create_actor_req>(arr);
                xDebug.Write($"actor id : {req.ActorId}");
                xDebug.Write($"actor acceleration : {req.CreateAcceleration}");
                xDebug.Write($"actor position : {req.CreatePosition}");
                xDebug.Write($"actor speed : {req.CreateSpeed}");
                xDebug.Write($"actor description : {req.ActorDescription}");

                // create response message
                create_actor_resp resp = new create_actor_resp
                {
                    Status = new status
                    { 
                        Info = { "create_actor_resp" }
                    },

                    ActorDescription = new actor_description
                    {
                        ActorType = "vehicle",
                        Length = 3f,
                        Width = 1.4f,
                        Height = 1.6f,
                    }
                };

                xDebug.Write("-> response msg : create_actor_resp");
                Response.SendData(id, resp);                
            }
            else if (id == SSP_MSG_ID.get_actor_state)
            {
                get_actors_states_req req = Converter.ToObject<get_actors_states_req>(arr);
                xDebug.Write(req.ActorsFilter.IdFilter.ActorsId.ToString());

                foreach(long actorId in req.ActorsFilter.IdFilter.ActorsId)
                {
                    // create response message
                    actor_state actorState = new actor_state();
                    actorState.ActorId = actorId;
                    actorState.Position = Converter.ToCoord6dof(0, 725.0, 0, 0, 0, 4.324567);
                    actorState.Speed = Converter.ToCoord6dof(-8.4678239, 0, 0, 0, 0, 0);
                    //actorState.Acceleration = Converter.ToCoord6dof(1, 1, 1, 2, 2, 2);                                        

                    get_actors_states_resp resp = new get_actors_states_resp();
                    resp.Status = new status()
                    {
                        Info = { "get_actors_states_resp" }
                    };
                    resp.ActorState.Add(actorState);

                    xDebug.Write("-> response msg : get_actors_states_resp");
                    Response.SendData(id, resp);
                }
            }
            else if (id == SSP_MSG_ID.set_weather)
            {
                set_weather_req req = Converter.ToObject<set_weather_req>(arr);
                xDebug.Write($"weather : {req.Weather}");

                set_weather_resp resp = new set_weather_resp();
                resp.Weather = req.Weather;

                resp.Status = new status()
                {
                    Info = { "set_weather_resp" }
                };

                xDebug.Write("-> response msg : set_weather_resp");
                Response.SendData(id, resp);
            }
            else if (id == SSP_MSG_ID.set_time_of_day)
            {
                set_time_of_day_req req = Converter.ToObject<set_time_of_day_req>(arr);
                xDebug.Write($"TimeOfDay : {req.TimeOfDay}");

                set_time_of_day_resp resp = new set_time_of_day_resp();
                resp.Status = new status()
                {
                    Info = { "set_time_of_day_resp" }
                };

                xDebug.Write("-> response msg : set_time_of_day_resp");
                Response.SendData(id, resp);
            }
            else if (id == SSP_MSG_ID.set_xy_trajectory_move)
            {
                set_xy_trajectory_move_req req = Converter.ToObject<set_xy_trajectory_move_req>(arr);
                xDebug.Write($"actor id : {req.ActorId}");
                xDebug.Write($"start_time_ms : {req.StartTimeMs}");
                xDebug.Write($"end_time_ms : {req.EndTimeMs}");

                int count = 1;
                foreach(coord_6dof pos in req.Polyline)
                {
                    xDebug.Write($"{count++} : {pos.ToString()}");
                }

                set_move_resp resp = new set_move_resp();
                resp.Status = new status
                { 
                    Info = { "set_move_resp" }
                };

                xDebug.Write("-> response msg : set_move_resp");
                Response.SendData(id, resp);
            }

        }
    }
}
