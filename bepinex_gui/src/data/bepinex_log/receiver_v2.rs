use crossbeam_channel::{Receiver, Sender};
use hashbrown::HashMap;
use std::thread;

use crate::data::bepinex_log::packet_protocol::*;

#[allow(non_upper_case_globals)]
pub static mut receiver: Option<LogReceiverV2> = Option::None;

#[derive(Clone)]
pub struct LogReceiverV2 {
    pub source_sender: Vec<LogSource>,
    pub source_receiver: Vec<Receiver<LogSource>>,
    pub log_senders: HashMap<LogSource, Sender<LogEvent>>,
    pub log_receiver: HashMap<LogSource, Receiver<LogEvent>>,
}

impl LogReceiverV2 {
    pub fn new() -> LogReceiverV2 {
        let (general_tab_mod_s, general_tab_mod_r): (Sender<LogEvent>, Receiver<LogEvent>) =
            crossbeam_channel::unbounded();
            
        let (console_tab_mod_s, console_tab_mod_r): (Sender<LogSource>, Receiver<LogSource>) =
            crossbeam_channel::unbounded();
            //hashbrown::HashSet
     }

    pub fn start_thread_loop(&self) {
        let inst = self.clone();
        thread::spawn(move || Self::thread_loop(inst));
    }

    #[allow(unreachable_code)]
    #[allow(unused_variables)]
    fn thread_loop(inst: LogReceiverV2) {}
}
