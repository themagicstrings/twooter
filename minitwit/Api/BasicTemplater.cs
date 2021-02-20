using System.Collections.Generic;
using Shared;
using System.Text;
using System.Linq;
using System;

namespace Api
{
  public class BasicTemplater
  {
    public static string Layout(
        string title = "Welcome",
        string body = "",
        UserReadDTO user = null,
        string[] flashes = default
    ) {
      StringBuilder sb = new StringBuilder();
      sb.Append($@"<!doctype html>
<head>
<title>{title} | MiniTwit</title>
<link rel=stylesheet type=text/css href=""/static/css/style.css"">
</head>
<body>
<div class=page>
  <h1>MiniTwit</h1>
  <div class=navigation>");

      if (user is null)
      sb.Append(@"
    <a href=""public_timeline"">public timeline</a> |
    <a href=""sign_up"">sign up</a> |
    <a href=""login"">sign in</a>");
    else sb.Append($@"
    <a href=""/{user.username}"">my timeline</a> |
    <a href=""/public"">public timeline</a> |
    <a href=""/logout"">sign out {user.username}</a>");

      sb.Append(@"</div>");

      if (flashes != null && flashes.Length > 0)
      {
        sb.Append("<ul class=flashes>");
        foreach (var message in flashes)
        {
          sb.Append($"<li>{message}</li>");
        }
        sb.Append("</ul>");
      }
      sb.Append($@"
  <div class=body>
  {body}
  </div>
  <div class=footer>
    MiniTwit &mdash; Not A Flask Application
  </div>
</div>
</body>
");
        return sb.ToString();
    }


    public static string GenerateTimeline(List<MessageReadDTO> messages, UserReadDTO user = null)
    {
      bool loggedin = user != null;
      messages.Sort((x, y) => DateTime.Compare(x.pub_date, y.pub_date));
      messages.Reverse();

      StringBuilder sb = new StringBuilder();
      sb.Append("<html>");
      if (loggedin) sb.Append("<form method=post action=add_message><input name=Text><input type=submit></form>");
      foreach (MessageReadDTO msg in messages)
      {
        sb.Append($"<p>{msg.author.username} [{msg.pub_date}]: {msg.text}</p>");
        if (loggedin) sb.Append($"<form method=post action={msg.author.username}/follow><button type=submit>Follow</button></form>");
      }
      sb.Append("</html>");
      return Layout(title: loggedin ? "Your timeline" : "Public timeline", body: sb.ToString(), user: user);
    }

    public static string GenerateLoginPage(UserReadDTO user = null)
    {
      return Layout(
      title: "Sign In",
      body: @"<h2>Sign In</h2>
      <form method=post action=login>
      <dl>
      <dt>Username:
      <dd><input type=text name=Username size=30>
      <dt>Password:
      <dd><input type=password name=Password size=30>
      </dl>
      <div class=actions><input type=submit value=""Sign In""></div>
                    </form>", user: user);
    }

    public static string GenerateRegisterPage(UserReadDTO user = null)
    {
      return Layout(
        title: "Sign Up",
        body: @"<h2>Sign Up</h2>
        <form method=post action=sign_up>
          <dl>
      <dt>Username:
      <dd><input type=text name=Username size=30>
      <dt>E-Mail:
      <dd><input type=text name=Email size=30>
      <dt>Password:
      <dd><input type=password name=Password1 size=30>
      <dt>Password <small>(repeat)</small>:
      <dd><input type=password name=Password2 size=30>
    </dl>
    <div class=actions><input type=submit value=""Sign Up""></div>
                    </form>",
        user
      );
    }
  }
}
