<img width="1400" alt="dotnetmauilogo" src="https://github.com/user-attachments/assets/51654efe-d874-4ab6-a32f-53a77ad4f083" />
—

This sample app is an open-source sample app, built with .NET MAUI. This app utilizes Descope and the Duende IdentityModel OidcClient library to perform a PKCE OAuth2/OIDC authentication flow.

Look at the [Duende.IdentityModel.OidcClient](https://docs.duendesoftware.com/identitymodel-oidcclient/) to learn more.

## Table of Contents 📝

1. [Features](#features)
2. [Installation](#installation)
3. [Running the Application](#running-the-application)
4. [Issue Reporting](#issue-reporting)
5. [License](#license)

## Features ✨

- **Descope Login / Logout**: Users can login and logout with their Descope credentials.
- **Secure Authentication Flow**: Secure PKCE flow using system browser.
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

3. Setup environment variables:

Fill in the **ProjectID** variable in **MauiProgram.cs** with your own Descope Project ID which can be found under [Project Settings](https://app.descope.com/settings/project), in the console.

In the Descope Console, navigate to [Project Settings](https://app.descope.com/settings/project) ➜ General ➜ Security ➜ Approved Domains, and add '**callback**' as an approved domain.

## Running the Application 🚀

To start the application, run:

```powershell
# Android
dotnet build -t:Run -f net8.0-android -p:Configuration=Debug

# Windows
dotnet build -t:Run -f net8.0-windows10.0.19041.0
```
Once the app launches, tap Log In ➜ sign in with Descope ➜ you’ll be returned to the app with your user claims displayed. Tap Log Out to clear the session.

## Issue Reporting ⚠️

This project is a simple demonstration of integrating Descope into a .NET MAUI application. For any issues or suggestions, feel free to open an issue in the GitHub repository.

## License 📜

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
