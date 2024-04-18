// build.rs

#[cfg(windows)]
fn main() {
    extern crate winres;

    /*
    csbindgen::Builder::default()
        .input_extern_file("../source/BepInEx.GUI/bepinex_gui/../backend/network/builder.rs")
        .input_extern_file(
            "../source/BepInEx.GUI/bepinex_gui/../backend/network/packet_protocol.rs",
        )
        .csharp_namespace("BepInEx.GUI.Bridge")
        .csharp_class_name("NativeMethods")
        .csharp_dll_name("BepInEx.GUI.Loader.Rust")
        .csharp_use_function_pointer(true)
        .generate_csharp_file("../BepInEx.GUI.Loader/src/NativeMethods.cs")
        .unwrap();
    */

    let mut res = winres::WindowsResource::new();
    res.set_icon("assets/icons/app_icon.ico");
    res.compile().unwrap();
}

#[cfg(unix)]
fn main() {
    todo!()
}
