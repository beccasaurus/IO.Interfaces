#! /bin/bash
rm -rf bin
rm -rf TestResult.xml
xbuild /p:Configuration=Release
nunit-color-console -labels "$@" bin/Release/IO.Interfaces.Specs.dll
