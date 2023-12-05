using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class MirPositionClass
{
    public int id ;
    public string ip ;
    public bool active ;
    public Status status;
    public string created_by ;
    public string description ;
    public int fleet_state ;
    public string robot_model ;
    public string created_by_id ;
    public string serial_number ;
    public int robot_group_id ;
    public string[] allowed_methods ;
    public string created_by_name ;
    public string fleet_state_text ;
    public Operation_Mode_Info operation_mode_info ;
    public bool factory_reset_needed ;
}

[Serializable]
public class Status
{
    public float moved ;
    public object[] errors ;
    public Header header ;
    public string map_id ;
    public int uptime ;
    public int mode_id ;
    public Position position;
    public int state_id ;
    public Velocity velocity ;
    public string footprint ;
    public Hook_Data hook_data ;
    public string mode_text ;
    public string robot_name ;
    public string session_id ;
    public string state_text ;
    public Hook_Status hook_status ;
    public User_Prompt user_prompt ;
    public string mission_text ;
    public string mode_key_state ;
    public int battery_voltage ;
    public int mission_queue_id ;
    public string software_version ;
    public int battery_percentage ;
    public bool safety_system_muted ;
    public bool unloaded_map_changes ;
    public int battery_time_remaining ;
    public float distance_to_next_target ;
    public string joystick_web_session_id ;
    public bool joystick_low_speed_mode_enabled ;
}


[Serializable]
public class Header
{
    public int seq ;
    public Stamp stamp ;
    public string frame_id ;
}

[Serializable]
public class Stamp
{
    public int secs ;
    public int nsecs ;
}

[Serializable]
public class Position
{
    public float x ;
    public float y ;
    public float orientation ;
}

[Serializable]
public class Velocity
{
    public float linear ;
    public float angular ;
}


[Serializable]
public class Hook_Data
{
    public Angle angle ;
    public int height ;
    public int length ;
    public int brake_state ;
    public int height_state ;
    public int gripper_state ;
}


[Serializable]
public class Angle
{
    public int angle ;
    public Timestamp timestamp ;
}

[Serializable]

public class Timestamp
{
    public int secs ;
    public int nsecs ;
}

[Serializable]
public class Hook_Status
{
    public Trolley trolley ;
    public bool available ;
    public bool trolley_attached ;
}

[Serializable]
public class Trolley
{
    public int id ;
    public int width ;
    public int height ;
    public int length ;
    public int offset_locked_wheels ;
}

[Serializable]
public class User_Prompt
{
    public string guid ;
    public object[] options ;
    public Timeout timeout ;
    public string question ;
    public string user_group ;
    public bool has_request ;
}

[Serializable]
public class Timeout
{
    public int secs ;
    public int nsecs ;
}

[Serializable]
public class Operation_Mode_Info
{
    public string mode_name ;
    public bool communicator_enabled ;
    public bool synchronization_enabled ;
    public bool charging_staging_enabled ;
    public bool mission_scheduling_enabled ;
    public bool collision_avoidance_enabled ;
    public bool resource_management_enabled ;
    public bool initial_synchronization_enabled ;
}
