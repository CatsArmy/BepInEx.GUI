// build.rs
#[cfg(windows)]
extern crate winres;

#[cfg(windows)]
fn main() {
    let csbindgenerator = csbindgen::Builder::default()
        // .input_extern_file("D:/source/BepInEx.GUI/bepinex_gui/src/lib/network/packet_protocol.rs")
        .input_extern_file("D:/source/BepInEx.GUI/bepinex_gui/src/data/bepinex_log/packet_protocol.rs")
        .csharp_namespace("BepInEx.GUI.Loader.Rust")
        .csharp_class_name("NativeMethods")
        .csharp_dll_name("bepinex_gui")
        .csharp_use_function_pointer(false)
        .generate_csharp_file("D:/source/BepInEx.GUI/BepInEx.GUI.Loader/src/NativeMethods.cs");
    
    if let Ok(_) = csbindgenerator {
        csbindgenerator.unwrap();
    }

    /*
    std::env::set_var("PROTOC", "D:/Downloads/protoc-26.1-win64/bin/protoc.exe");
     let tonic_build = tonic_build::configure()
         .build_client(true)
         .build_server(true)
         .build_transport(true)
         .out_dir("src/codegen/");
    let _ = tonic_build.compile(&["src/proto/packet.proto"], &["src/proto"]);
    */

    let mut res = winres::WindowsResource::new();
    res.set_icon("src/assets/icons/app_icon.ico");
    //res.set_icon("D:/source/BepInEx.GUI/bepinex_gui/src/assets/icons/app_icon.ico");
    res.compile().unwrap();
}

#[cfg(unix)]
fn main() {
    todo!()
}
