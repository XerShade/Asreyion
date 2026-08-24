---
title: "Account Sign-in and Registration Is Live"
date: 2026-08-24 06:57:48
categories: 
  - "Updates"
tags: 
  - "account"
  - "sign-in"
  - "registration"
  - "are"
  - "live"
---

### Account Sign-in and Registration Is Live
The account sign-in and registration features are now live. I am using external authentication providers for sign-in, which means you can use your existing accounts from platforms like Google, Facebook, or GitHub to log in. This simplifies the process and enhances security as there are no passwords being collected on my website that could get exposed if my database gets compromised somehow.

There's not much that you can do with an account yet, but I have deployed the ability to sign in and register. I will be adding more features to the account system in the future, such as the ability to manage your account, change your email, and more. For now we only have Google Authentication but I plan to add more services once I get the back end code for the authentication system more robust and extendable as right now I have to manually code all the UI and back end parts for each provider manually.

Please note that when you sign in with an external provider, I will not have access to your password or any sensitive information. The authentication process is handled securely by the provider, and I only receive a token that confirms your identity.

We also have a password login system in place, but that is intended for staff and admin users only in case an external authentication provider is not available. If you are a regular user, please use the external authentication providers to sign in and ignore the password login options. You also do not need to link to an existing account, all you need to do is sign in with your preferred provider and an account will be created for you automatically. If you want to add multiple external authentication providers, you can do so by logging into your account and going to your account settings to manage external provider connetions. The link to existing account system is intended for staff use when they're working with new providers or testing things.

Anyways that's all for now, I just wanted to make a quick post to let everyone know the website is now live with account sign-in and registration features. I will be adding more features to the account system in the future, so stay tuned for updates.

Thanks for reading,  
-XerShade