use std::path::PathBuf;

use sysinfo::Pid;

use crate::app;

const DEBUG: bool = false;
const DEFAULT_PID: usize = 17584;
const DEFUALT_SOCKET_PORT: u16 = 27090;

pub struct AppLaunchConfig {
    process_name: String,
    game_folder_full_path: PathBuf,
    bepinex_log_output_file_full_path: PathBuf,
    bepinex_gui_csharp_cfg_full_path: PathBuf,
    target_process_id: Pid,
    log_socket_port_receiver: u16,
    icon_path: String,
    window_title: String,
}

impl AppLaunchConfig {
    const ARG_COUNT: usize = 9;

    pub fn from(args: &Vec<String>) -> Option<Self> {
        if args.len() == Self::ARG_COUNT {
            let bepinex_version = &args[1];
            let process_name = &args[2];
            let window_title = app::NAME.to_owned() + " " + bepinex_version + " - " + process_name;

            Some(Self {
                process_name: process_name.into(),
                game_folder_full_path: (&args[3]).into(),
                bepinex_log_output_file_full_path: (&args[4]).into(),
                bepinex_gui_csharp_cfg_full_path: (&args[5]).into(),
                target_process_id: args[6].parse::<Pid>().unwrap(),
                log_socket_port_receiver: args[7].parse::<u16>().unwrap(),
                icon_path: String::from(&args[8]),
                window_title,
            })
        } else {
            tracing::error!("Problem with args {:?} {:?}", args.len(), args);

            None
        }
    }
    ///there is no reason to defualt to Risk Of Rain 2 and steam as it is not allways going to exist in the C Drive or any drive as a matter of fact
    ///therfore you cant be sure the user even has it installed. although it is a pretty safe bet to assume that steam exists im pretty sure.
    pub fn default() -> Self {
        let bepinex_version_string = "3.1.0";
        let process_name = "BepInEx Console GUI";
        if DEBUG {
            return Self {
                process_name: String::from("Lethal Company"),
                game_folder_full_path: "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Lethal Company".into(),
                bepinex_log_output_file_full_path: "C:\\Program Files (x86)\\r2modmanPlus-local\\LethalCompany\\profiles\\Default\\BepInEx\\LogOutput.log".into(),
                bepinex_gui_csharp_cfg_full_path: "C:\\Program Files (x86)\\r2modmanPlus-local\\LethalCompany\\profiles\\Default\\BepInEx\\config\\BepInEx.GUI.cfg".into(),
                target_process_id: Pid::from(24988),//CHANGE ON DEBUG
                log_socket_port_receiver: 51730,//CHANGE ON DEBUG
                icon_path: String::from("None"),//Maybe change
                window_title: app::NAME.to_owned() + " " + bepinex_version_string + " - " + process_name,
            }
        }
        return Self {
            process_name: process_name.into(),
            game_folder_full_path: "C:\\Program Files (x86)".into(),
            bepinex_log_output_file_full_path: "C:\\Program Files (x86)".into(),
            bepinex_gui_csharp_cfg_full_path: "C:\\Program Files (x86)".into(),
            target_process_id: Pid::from(DEFAULT_PID),
            log_socket_port_receiver: DEFUALT_SOCKET_PORT,
            icon_path: String::from("None"),
            window_title : app::NAME.to_owned() + " " + bepinex_version_string + " - " + process_name,
        }
    }

    pub fn process_name(&self) -> &str {
        self.process_name.as_ref()
    }

    pub const fn game_folder_full_path(&self) -> &PathBuf {
        &self.game_folder_full_path
    }

    pub const fn bepinex_log_output_file_full_path(&self) -> &PathBuf {
        &self.bepinex_log_output_file_full_path
    }

    pub const fn bepinex_gui_csharp_cfg_full_path(&self) -> &PathBuf {
        &self.bepinex_gui_csharp_cfg_full_path
    }

    pub const fn target_process_id(&self) -> Pid {
        self.target_process_id
    }

    pub const fn log_socket_port_receiver(&self) -> u16 {
        self.log_socket_port_receiver
    }

    pub fn app_icon_path(&self) -> &String {
        &self.icon_path
    }

    pub fn window_title(&self) -> &str {
        self.window_title.as_ref()
    }
}
