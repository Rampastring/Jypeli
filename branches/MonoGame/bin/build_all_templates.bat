@echo off
setlocal
call build_project_templates ..\Projektimallit\VS2012
call build_project_templates ..\Projektimallit\WP8
call build_project_templates ..\Projektimallit\Win8
endlocal
