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
    <a href=""public"">public timeline</a> |
    <a href=""sign_up"">sign up</a> |
    <a href=""login"">sign in</a>");
    else sb.Append($@"
    <a href=""/"">my timeline</a> |
    <a href=""/public"">public timeline</a> |
    <a href=""/logout"">sign out [{user.username}]</a>");

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

    private static string noMessagesHtml = @"<ul class=""messages""> 
          <li><em>There's no messages so far.</em></li>
          </ul>
        ";

    public static string GenerateTimeline(List<MessageReadDTO> messages, UserReadDTO user = null, bool isPublicTimeline = false)
    {
      bool loggedin = user != null;
      messages.Sort((x, y) => DateTime.Compare(x.pub_date, y.pub_date));
      messages.Reverse();

      StringBuilder sb = new StringBuilder();
      // sb.Append("<html>");
      if (isPublicTimeline) sb.Append("<h2>Public Timeline</h2>");
      else sb.Append("<h2>My Timeline</h2>");
      if (loggedin && !isPublicTimeline) sb.Append(
        $@"<div class=""twitbox""><h3>What's on your mind {user.username}?</h3>
        <form action=""/add_message"" method=""post""><p><input type=""text"" name=""text"" size=""60""><input type=""submit"" value=""Share""></p></form></div>"
        // "<form method=post action=add_message><input name=Text><input type=submit></form>"
        );
      if (messages.Count == 0) 
      {
        sb.Append(noMessagesHtml);
      }
      foreach (MessageReadDTO msg in messages)
      {
        sb.Append($"<p>{msg.author.username} [{msg.pub_date}]: {msg.text}</p>");
        if (loggedin) sb.Append($"<form method=post action={msg.author.username}/follow><button type=submit>Follow</button></form>");
      }
      // sb.Append("</html>");
      return Layout(title: loggedin ? "Your timeline" : "Public timeline", body: sb.ToString(), user: user);
    }
    

    public static string GenerateLoginPage(UserReadDTO user = null)
    {
      return Layout(
      title: "Sign In | MiniTwit",
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
        title: "Sign Up | MiniTwit",
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
