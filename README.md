<img width="1400" alt="dotnetmauilogo" src="https://github.com/user-attachments/assets/51654efe-d874-4ab6-a32f-53a77ad4f083" />


This sample app is an open-source sample app, built with .NET MAUI. It uses the [Descope.Maui](https://github.com/descope/descope-maui) SDK to add Descope authentication to iOS and Android apps.

## Table of Contents 📝

1. [Features](#features)
2. [Installation](#installation)
3. [Running the Application](#running-the-application)
4. [Issue Reporting](#issue-reporting)
5. [License](#license)

## Features ✨

- **Descope Login / Logout**: Users can login and logout with their Descope credentials.
- **Secure Authentication Flow**: Secure PKCE flow using system browser.
- **Session Management**: The session is saved securely, restored on the next launch, and refreshed automatically.
- **User Dashboard**: After logging in, users are redirected to a dashboard which displays their session information.
- **Responsive UI** – Login screen vs. authenticated dashboard swap.

## Installation 💿

1. Clone the repository:

```powershell
git clone https://github.com/descope-sample-apps/dotnet-maui-sample-app.git
cd dotnet-maui-sample-app
```

2. Restore NuGet packages:

```powershell
dotnet restore
```
3. Install necessary workloads (requires the .NET 10 SDK)

```powershell
dotnet workload install maui
```

4. Configure your Descope project:

In **MauiProgram.cs**, replace `<YOUR_DESCOPE_PROJECT_ID>` with your Descope Project ID, which can be found under [Project Settings](https://app.descope.com/settings/project) in the console. The sample uses the redirect URI `descopemaui://auth`. If you change its scheme, also change the **DataScheme** in **Platforms/Android/WebAuthenticatorCallbackActivity.cs** and the **CFBundleURLSchemes** entry in **Platforms/iOS/Info.plist**.

If your project has [Approved Domains](https://app.descope.com/settings/project) set, add the exact redirect URI (`descopemaui://auth`), since wildcards don't work for it. If **Apply Trusted Domains on Flow Execution** is on, also add `api.descope.com` (or your custom domain).

## Running the Application 🚀

To start the application, run:

```powershell
# Android
dotnet build -t:Run -f net10.0-android

# iOS simulator
dotnet build -t:Run -f net10.0-ios
```
Once the app launches, tap Log In ➜ sign in with Descope ➜ you’ll be returned to the app with your user claims displayed. Tap Log Out to clear the session.

## Issue Reporting ⚠️

This project is a simple demonstration of integrating Descope into a .NET MAUI application. For any issues or suggestions, feel free to open an issue in the GitHub repository.

## License 📜

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
