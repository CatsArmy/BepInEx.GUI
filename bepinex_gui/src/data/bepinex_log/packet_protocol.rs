use crate::data::bepinex_log::receiver_v2::receiver;

#[no_mangle]
pub extern "C" fn send(log: LogEventRaw) {
    let log_event = log.to_log_event();
    unsafe {
        let r = receiver.expect("msg");
        let ss = r.source_sender;
        let sr = r.source_receiver;
        let ls = r.log_senders;
        let lr = r.log_receiver;
        let mut f = false;
        ss.iter().for_each(move |source| {
            if source.source == log_event.source.source {
                f = true;
            }
        });
        if !f {
            //add it
            //too complex for brain need to rethink or get help
            //im ded 💀☠️☠️☠️☠️
        }
    }
}

#[no_mangle]
pub unsafe fn csharp_to_rust_utf8(utf8_str: *const u8, utf8_len: i32) -> String {
    let slice = std::slice::from_raw_parts(utf8_str, utf8_len as usize);
    String::from_utf8_unchecked(slice.to_vec())
}

#[repr(C)]
#[derive(Clone)]
#[allow(non_camel_case_types)]
pub struct Utf8Str_cs {
    utf8_str: *const u8,
    utf8_len: i32,
}

impl Utf8Str_cs {
    fn to_string(self) -> String {
        unsafe {
            return csharp_to_rust_utf8(self.utf8_str, self.utf8_len);
        }
    }
}

#[repr(C)]
pub struct LogEventRaw {
    pub data: Utf8Str_cs,
    pub level: BepInExLogLevel,
    pub source: LogSourceRaw,
}

impl LogEventRaw {
    pub fn to_log_event(self) -> LogEvent {
        LogEvent {
            data: self.data.to_string(),
            level: self.level,
            source: self.source.to_log_source(),
            is_selected: false,
        }
    }
}

#[repr(C)]
#[derive(Clone)]
pub struct LogSourceRaw {
    pub source: Utf8Str_cs,
    pub version: Version,
}

impl LogSourceRaw {
    pub fn to_log_source(self) -> LogSource {
        LogSource {
            source: self.source.to_string(),
            version: self.version,
        }
    }
}

#[repr(C)]
#[derive(Clone)]
pub struct Version {
    pub major: i32,
    pub minor: i32,
    pub patch: i32,
}

#[derive(Clone)]
pub struct LogSource {
    pub source: String,
    pub version: Version,
}

#[derive(Clone)]
pub struct LogEvent {
    pub data: String,
    pub level: BepInExLogLevel,
    pub source: LogSource,
    pub is_selected: bool,
}

#[repr(u8)]
#[derive(Clone)]
pub enum BepInExLogLevel {
    //  No level selected.
    None = 0,

    //  A fatal error has occurred, which cannot be recovered from.
    Fatal = 1,

    //  An error has occured, but can be recovered from.
    Error = 2,

    //  A warning has been produced, but does not necessarily mean that something wrong has happened.
    Warning = 4,

    //  An important message that should be displayed to the user.
    Message = 8,

    //  A message of low importance.
    Info = 16,

    //  A message that would likely only interest a developer.
    Debug = 32,

    //  All log levels.
    All = 64,
}
