using System;
using System.Collections;
using System.IO;
using System.Text;
using Morai.Protobuf.Foretify;
using UnityEngine;

namespace ForetifyLinker
{
    public class MyTransform
    {
        public void Create(double x, double y, double z, double roll, double pitch, double yaw)
        {
            position = new Vector3((float)x, (float)y, (float)z);
            rotation = new Vector3((float)roll, (float)pitch, (float)yaw);
        }

        public Vector3 position { get; set; }
        public Vector3 rotation { get; set; }
    }

    public class Receiver : IReceiver
    {
        public IResponse Response { get; set; }
        public IDebug xDebug { get; set; }

        private bool isFirst = true;

        private coord_6dof actorPos = new coord_6dof();
        private coord_6dof actorSpeed = new coord_6dof();

        LockQueue<MyTransform> buffers = new LockQueue<MyTransform>();

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

                actorPos = req.CreatePosition;
                actorSpeed = req.CreateSpeed;

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

                double x = actorPos.X == null ? 0 : actorPos.X.Value;
                double y = actorPos.Y == null ? 0 : actorPos.Y.Value;
                double z = actorPos.Z == null ? 0 : actorPos.Z.Value;

                ForetifyManager.Instance.CreateActor(req.ActorId.ToString(), x, y, z);

                xDebug.Write("-> response msg : create_actor_resp");
                Response.SendData(id, resp);
            }
            else if (id == SSP_MSG_ID.get_actor_state)
            {
                get_actors_states_req req = Converter.ToObject<get_actors_states_req>(arr);
                xDebug.Write(req.ActorsFilter.IdFilter.ActorsId.ToString());

                //foreach (long actorId in req.ActorsFilter.IdFilter.ActorsId)
                {
                    // create response message
                    actor_state actorState = new actor_state();
                    actorState.ActorId = req.ActorsFilter.IdFilter.ActorsId[0];

                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        MyTransform tr = buffers.Dequeue();
                        actorPos = Converter.ToCoord6dof(tr.position.x, tr.position.y, tr.position.z, tr.rotation.x, tr.rotation.y, tr.rotation.z);
                    }

                    actorState.Position = actorPos;
                    xDebug.Write($"actor pos : {actorPos}");
                    actorState.Speed = Converter.ToCoord6dof(-8.4678239, 0, 0, 0, 0, 0);

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
                try
                {
                    set_xy_trajectory_move_req req = Converter.ToObject<set_xy_trajectory_move_req>(arr);
                    xDebug.Write($"actor id : {req.ActorId}");
                    xDebug.Write($"start_time_ms : {req.StartTimeMs}");
                    xDebug.Write($"end_time_ms : {req.EndTimeMs}");

                    int count = 1;
                    foreach (coord_6dof pos in req.Polyline)
                    {
                        xDebug.Write($"{count++} : {pos.ToString()}");
                        MyTransform tr = new MyTransform();
                        double x = pos.X == null ? 0 : pos.X.Value;
                        double y = pos.Y == null ? 0 : pos.Y.Value;
                        double z = pos.Z == null ? 0 : pos.Z.Value;
                        double roll = pos.Roll == null ? 0 : pos.Roll.Value;
                        double pitch = pos.Pitch == null ? 0 : pos.Pitch.Value;
                        double yaw = pos.Yaw == null ? 0 : pos.Yaw.Value;

                        tr.Create(x, y, z, roll, pitch, yaw);                        
                        buffers.Enqueue(tr);
                    }

                    set_move_resp resp = new set_move_resp();
                    resp.Status = new status
                    {
                        Info = { "set_move_resp" }
                    };

                    xDebug.Write("-> response msg : set_move_resp");
                    Response.SendData(id, resp);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }                
            }
            else if (id == SSP_MSG_ID.start_step)
            {
                start_step_req req = Converter.ToObject<start_step_req>(arr);

                xDebug.Write("start_step_req");
                xDebug.Write($"RequestedSimTimeMs : {req.RequestedSimTimeMs}");

                // create response message
                start_step_resp resp = new start_step_resp
                {
                    Status = new status
                    {
                        Info = { "start_step_resp" },
                    }
                };
                xDebug.Write("-> response msg : start_step_resp");
                Response.SendData(id, resp);
            }
            else if (id == SSP_MSG_ID.wait_step)
            {
                wait_step_req req = Converter.ToObject<wait_step_req>(arr);

                xDebug.Write("wait_step_req");
                xDebug.Write($"MaxWaitMs : {req.MaxWaitMs}");

                // create response message
                wait_step_resp resp = new wait_step_resp
                {
                    Status = new status
                    {
                        Info = { "wait_step_resp" },
                    }
                };
                xDebug.Write("-> response msg : wait_step_resp");
                Response.SendData(id, resp);
            }
        }
    }
}
