
namespace ForetifyLinker
{
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
}
