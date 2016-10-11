
# running notebin

running notebin is very easy and straightforward.
dotnet should run on any platform that supports .net core,
including `windows`, `macos`, and most `linux` systems.

## prerequisites

- [dotnet core tools](https://dot.net)

## steps

1. clone the notebin repository
1. download submodules with `git submodule update --init --recursive`
1. change to the root of the cloned repository
1. run `dotnet restore`. this should set up everything so notebin is ready to run
1. change to the `src/NoteBin3` directory
1. optionally edit `hosting.json` to set the server urls
1. `dotnet run` will start the application