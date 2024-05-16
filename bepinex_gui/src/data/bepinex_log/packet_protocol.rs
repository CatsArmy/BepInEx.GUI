use std::cell::RefCell;

use super::receiver_v2::LogReceiverV2;

#[allow(non_upper_case_globals)]
pub static mut receiver: Option<RefCell<LogReceiverV2>> = Option::None;

#[no_mangle]
pub extern "C" fn send(log: LogEventRaw) {
    unsafe {
        if let Some(r) = receiver.as_ref() {
        //.expect("expected reciver to be a some");
        let mut r = r.to_owned();
        let r = r.get_mut();
            r.send(log.to_log_event());
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
    pub utf8_str: *const u8,
    pub utf8_len: i32,
}

#[repr(C)]
pub struct LogEventRaw {
    pub data: Utf8Str_cs,
    pub level: BepInExLogLevel,
    pub source: LogSourceRaw,
}

#[repr(C)]
#[derive(Clone)]
pub struct LogSourceRaw {
    pub source: Utf8Str_cs,
    pub version: Version,
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
