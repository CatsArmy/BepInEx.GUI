// build.rs

#[cfg(windows)]
extern crate winres;

#[cfg(windows)]
fn main() -> Result<(), Box<dyn std::error::Error>> {
    std::env::set_var("PROTOC", "D:/Downloads/protoc-26.1-win64/bin/protoc.exe");
     let tonic_build = tonic_build::configure()
         .build_client(true)
         .build_server(true)
         .build_transport(true)
         .out_dir("src/codegen/");
    let _ = tonic_build.compile(&["src/proto/packet.proto"], &["src/proto"]);

    let mut res = winres::WindowsResource::new();
    res.set_icon("assets/icons/app_icon.ico");
    res.compile().unwrap();
    
    Ok(())
}

#[cfg(unix)]
fn main() {
    todo!()
}
