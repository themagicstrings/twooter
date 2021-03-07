
using System.Collections.Generic;
using Shared;
using System.Text;
using System.Linq;
using System;

namespace Api
{
  public class BasicTemplater
  {
    public static List<string> flashes = new List<string>();
    public static List<string> errors = new List<string>();
    public static string Layout(
        string title = "Welcome",
        string body = "",
        UserReadDTO user = null
    ) {
      StringBuilder sb = new StringBuilder();
      sb.Append($@"<!doctype html>
<head>
<title>{title} | MiniTwit</title>
<link rel=stylesheet type=text/css href=""/static/css/style.css"">
<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
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
    <form method=post action=/logout  style=""display: inline-block""><button class=nav_a type=submit>sign out [{user.username}]</button></form>");

      sb.Append(@"</div>");

      
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

    public static StringBuilder addNotifications(StringBuilder sb)
    {
      if (flashes != null && flashes.Count > 0)
      {
        sb.Append("<ul class=flashes>");
        foreach (var message in flashes)
        {
          sb.Append($"<li>{message}</li>");
        }
        sb.Append("</ul>");
      }
      if (errors.Count > 0)
      {
        
        foreach (var error in errors)
        {
          sb.Append($@"<div class=""error""><strong>Error: </strong>{error}</div>");
        }
        
      }
      return sb;
    }

    public static string GenerateTimeline(List<MessageReadDTO> messages, timelineType type, UserReadDTO user = null, string otherPersonUsername = "")
    {
      bool loggedin = user != null;
      messages.Sort((x, y) => DateTime.Compare(x.pub_date, y.pub_date));
      messages.Reverse();

      StringBuilder sb = new StringBuilder();

      if (type == timelineType.PUBLIC) sb.Append("<h2>Public Timeline</h2>");
      else if (type == timelineType.OTHER) sb.Append($"<h2>{otherPersonUsername}'s Timeline</h2>");
      else if (type == timelineType.SELF) sb.Append($"<h2>My Timeline</h2>");

      sb = addNotifications(sb);

      if (loggedin && type == timelineType.SELF) sb.Append(
        $@"<div class=""twitbox""><h3>What's on your mind {user.username}?</h3>
        <form action=""/add_message"" method=""post""><p><input type=""text"" name=""text"" size=""60""><input type=""submit"" value=""Share""></p></form></div>"
        );
      if (messages.Count == 0)
      {
        sb.Append(@"<ul class=""messages"">
          <li><em>There's no messages so far.</em></li>
          </ul>
        ");
      }
      else
      {
        sb.Append(@"<ul class=""messages"">");
        foreach (MessageReadDTO msg in messages)
        {
          string optionalZero = msg.pub_date.Month < 10 ? "0" : "";
          var reformattedDateTime = "- " + msg.pub_date.Year + "-" + optionalZero + msg.pub_date.Month + "-" + msg.pub_date.Day + " @ " + msg.pub_date.Hour + ":" + msg.pub_date.Minute;
          // sb.Append($"<p>{msg.author.username} [{msg.pub_date}]: {msg.text}</p>");
          sb.Append($@"
          <li>

            <p>
              <strong>
                <a href=""/{msg.author.username}"">{msg.author.username}</a>
              </strong>
              {msg.text}
              <small>
                {reformattedDateTime}
              </small>
            </p>
          </li>");
          // if (loggedin)
          // {
          //   sb.Append($"<form method=post action={msg.author.username}/follow><button type=submit>Follow</button></form>");
          // }
        }
        sb.Append("</ul>");
      }
      string toReturn = Layout(title: loggedin ? "Your Timeline" : "Public Timeline", body: sb.ToString(), user: user);
      clearNotifications();
      return toReturn;
    }


    public static string GenerateLoginPage(UserReadDTO user = null)
    {
      StringBuilder sb = new StringBuilder();

      sb.Append("<h2>Sign In</h2>");
      sb = addNotifications(sb);
      sb.Append(@"<form method=post action=login><dl><dt>Username:<dd><input type=text name=Username size=30><dt>Password:<dd><input type=password name=Password size=30></dl>
      <div class=actions><input type=submit value=""Sign In""></div>
                    </form>");

      string toReturn = Layout(
      title: "Sign In | MiniTwit",
      body: sb.ToString()
      , user: user);
      clearNotifications();
      return toReturn;
    }

    public static string GenerateRegisterPage(UserReadDTO user = null)
    {

      StringBuilder sb = new StringBuilder();
      sb.Append("<h2>Sign Up</h2>");
      sb = addNotifications(sb);
      sb.Append(@"<form method=post action=sign_up>
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
                    </form>");

      string toReturn =  Layout(
        title: "Sign Up | MiniTwit",
        body: sb.ToString(),
        user
      );
    clearNotifications();
    return toReturn;
    }

    internal static string Generate404Page(UserReadDTO user)
    {
      return @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 3.2 Final//EN"">
<title>404 Not Found</title>
<h1>Not Found</h1>
<p>The requested URL was not found on the server. If you entered the URL manually please check your spelling and try again.</p>
      ";
    }

    private static void clearNotifications()
    {
      flashes.Clear();
      errors.Clear();
    }
  }

  public enum timelineType
  {
    SELF, PUBLIC, OTHER
  }
}
