use crossbeam_channel::{Receiver, Sender};
use std::{cell::RefCell, thread};

use crate::data::{self, bepinex_log::packet_protocol::*};

#[derive(Clone)]
pub struct LogReceiverV2 {
    pub log_events: RefCell<hashbrown::HashMap<LogSource, LogEvent>>,
    pub log_senders: Sender<LogEvent>,
    pub log_receiver: Receiver<LogEvent>,
}

impl LogReceiverV2 {
    pub fn new() -> LogReceiverV2 {
        let (s, r): (Sender<LogEvent>, Receiver<LogEvent>) = crossbeam_channel::unbounded();

        let mut map = hashbrown::HashMap::new();
        let mut mapa: hashbrown::HashMap<LogSource, LogEvent> = hashbrown::HashMap::default();


        let log_receiver = LogReceiverV2 {
            log_events: RefCell::from(map),
            log_senders: s,
            log_receiver: r,
        };
        unsafe {
            let cell = RefCell::from(log_receiver.clone());
            data::bepinex_log::packet_protocol::receiver = Some(cell);
        }
        return log_receiver;
    }

    pub fn send(&mut self, event: LogEvent) {
        let log_events = self.log_events.borrow_mut();

        if !(log_events
            .keys()
            .any(|log| log.source == event.source.source))
        {
            
        } else {
        }
    }

    pub fn start_thread_loop(&self) {
        let inst = self.clone();
        thread::spawn(move || Self::thread_loop(inst));
    }

    #[allow(unreachable_code)]
    #[allow(unused_variables)]
    fn thread_loop(inst: LogReceiverV2) {}
}
