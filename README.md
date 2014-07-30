Getting Started
====================

Starting framework code for Unity ports to Windows.

You can view an overview of the architecture here:
https://www.dropbox.com/s/w8wt0au5602vl57/MarkerMetro.Unity.WinShared.jpg

Configured and tested with: Unity 4.5f6, and Unity 4.5.1f3

The approach when starting a new porting project is to simply copy and paste the necessary folders across to the client repo as detailed below:


Build Server Setup
==================
It's important to copy the build steps and tweak for your project as required, see the steps here:

http://mmbuild2.markermetro.com:9091/admin/editBuildRunners.html?id=buildType:MarkerMetroUnityWinShared_CI
http://mmbuild2.markermetro.com:9091/admin/editBuildRunners.html?id=buildType:MarkerMetroUnityWinShared_Release


The Unity Project
==================

The root of the repo is a small Unity project for testing purposes.

Copy and paste the following /Assets subfolders and files and the .gitignore to the existing Assets folder in client repo's Unity project (normally on root but could be in a sub folder)
Assets/MetroTestCertificate.pfx
Assets/MetroTestCertificate.pfx.meta
Assets/Editor/*
Assets/Plugins/*

The code here helps with automated builds and includes various helper files.

You can build out this project to the WindowsSolution folders as below, note that you should ensure the ProductName in PlayerSettings is "UnityProject" so that everything just works.

The Windows Solution
====================
This is the base WindowsSolution folder to be used for all Unity Projects. Copy it across to any new project and add to the root of the client repo. This contains all the goodness from working on previous projects at the app level.

Run the Init.ps1 script on the root to rename UnityProject to your product name (e.g LostLight). 

Then subsequently build out from Unity to /WindowsSolution/WindowsStore and /WindowsSolution/WindowsPhone.

Note that visual assets are from the Disney BOLA game so you know exactly which ones to replace.

App Name Localization
======================

Windows Store app manifest uses AppName and AppDescription resources to localize the store name, application name and description.

Windows Phone manifest uses a build-process generated AppResLib.dll[*.mui] files that pull strings from resources automatically:
- ApplicationTitle is used to generate resource string 100, wich is used to Application Display Name
- ApplicationTileTitle is used to generate resource string 101, wich is used to set Tile Title in manifest
- ApplicationDescription is used to generate resource string 102, which is used to set Application Description

If any of these strings are missing from resources, AppResLibGenerator will report a warning.

AppResLibGenerator is referenced as [Nuget Package](https://www.nuget.org/packages/MarkerMetro.WindowsPhone.AppResLibGenerator/) and is also on [Github](https://github.com/MarkerMetro/AppResLibGenerator)
 
Submission To Store
=====================
Both the Windows Phone and Windows Store apps are submitted to the actual stores. 

To access portals use http://dev.windows.com/ (markermetro@live.com account, yell if you need access)

Windows Phone App: http://www.windowsphone.com/s?appid=3d4131e7-bc32-4688-a486-e3ee6d2310cb
Windows Store App: [link]

They are submitted using the markermetro@live.com developer account and allow us to test out features not otherwise possible in development. For example, application name localization in the store.

Unity Plugins from NuGet
=====================================================================
This is the Nuget folder allowing for easy plugin integration to your Unity project. Copy it across to any new project and add to the root of the client repo's Unity project (normally on root but could be in a sub folder)

By default core Marker Metro plugins are included (WinLegacy, WinIntegration and LitJson), but the .csproj file can be edited to maintain plugin list.

To add/update the plugins you can run the following: \NuGet\Update_NuGet_Packages.bat (ensuring you have set up NuGet Access).

Once you have done this, be sure and push the updates to the dependencies.

If you need to work on any of the dependencies, you will need to open the project from Marker Metro Github and push any changes.

Once you have made the changes, you can manually run a build on the build server (See Automated Builds below)

Once the build has been run, you can then run the bat file above to include the latest binaries.

Windows Phone Low Memory Optimization
====================

There is a script that tries to optimize assets settings to lower memory usage, which is useful specially for Windows Phone 8.
You can find it at `\Assets\Editor\MarkerMetro\MemoryOptimizer.cs`.
Please refer to the code documentation for instructions on how to use it.

First Time Marker Metro NuGet Access
=====================================

Use  Marker Metro's private [NuGet](http://docs.nuget.org/docs/start-here/installing-nuget) feed: 
[http://mmbuild1.cloudapp.net/httpAuth/app/nuget/v1/FeedService.svc/](http://mmbuild1.cloudapp.net/httpAuth/app/nuget/v1/FeedService.svc/)
If you don't have personal account you can always use Disney's guest account:

*Username*: Disney

*Password*: Disney40cks

This project repository incudes a NuGet folder in the root with *nuget.exe* and it can be used to setup sources and store passwords. To add Marker Metro's Private Feed and remember authentication you can use following command-line:

**./NuGet.exe sources add -Name "Marker Metro Private" -Source "http://mmbuild1.cloudapp.net/httpAuth/app/nuget/v1/FeedService.svc/" -UserName disney -Password Disney40cks**

You can also modify previously added feed using update command:

**./NuGet.exe sources update -Name "Marker Metro Private" -UserName disney -Password Disney40cks**

To list existing sources you can use:

**./NuGet.exe sources**


