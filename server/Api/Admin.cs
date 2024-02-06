//API bits for the admin CLI//public endpoints for subscribe/unsubscribe
using Microsoft.AspNetCore.Mvc;
using Tailwind.Data;
using Tailwind.Mail.Commands;
using Tailwind.Mail.Models;

namespace Tailwind.Mail.Api;

public class ValidationResponse{
  public bool Valid { get; set; }
  public string Message { get; set; }
  public long Contacts { get; set; }
  public MarkdownEmail? Data { get; set; }
  public ValidationResponse()
  {
    Message = "The markdown is valid";
  }
}
public class ValidationRequest{
  public string? Markdown { get; set; }
}

public class QueueBroadcastResponse{
  public bool Success { get; set; }
  public string? Message { get; set; }
  public CommandResult? Result { get; set; }
}

public class Admin{

  //all of these routes will be protected in some way...
  public static void MapRoutes(IEndpointRouteBuilder app)
  {
    //queue up a broadcast
    //CRUD for contacts
    //CRUD for email templates
    //Message queue problems - failed, bounced
    //Message queue pending
    //Broadcast summary
    //Contact stats
    //Tag stats
    //validate a broadcast
    app.MapPost("/admin/queue-broadcast", ([FromBody] ValidationRequest req) => {
      var mardown = req.Markdown;
      var doc = MarkdownEmail.FromString(req.Markdown);
      //this should already be validated but...
      if(!doc.IsValid()){
        return new QueueBroadcastResponse{
          Success = false,
          Message = "Ensure there is a Body, Subject and Summary in the markdown",
        };
      }
      var broadcast = Broadcast.FromMarkdownEmail(doc);
      var res = new CreateBroadcast(broadcast).Execute();
      //ensure that it has a subject, summary, and slug
      var response = new QueueBroadcastResponse{
        Success = res.Inserted > 0,
        Message = $"The broadcast was queued with ID {res.Data.BroadcastId} and {res.Inserted} messages were created",
        Result = res
      };

      return response;
    }).WithOpenApi(op => {
      op.Summary = "Queue a broadcast for your contacts";
      op.Description = "This pops the messages for your broadcast into the queue and double checks the validation";
      op.RequestBody.Description = "The markdown for the email";
      return op;
    });
    app.MapPost("/admin/validate", ([FromBody] ValidationRequest req) => {
      if(req.Markdown == null){
        return new ValidationResponse{
          Valid = false,
          Message = "The markdown is null"
        };
      }
      var doc = MarkdownEmail.FromString(req.Markdown);
      if(!doc.IsValid()){
        return new ValidationResponse{
          Valid = false,
          Message = "Ensure there is a Subject and Summary in the markdown",
          Data = doc
        };
      }
      var broadcast = Broadcast.FromMarkdownEmail(doc);
      //how many contacts?
      var contacts = broadcast.ContactCount();
      //ensure that it has a subject, summary, and slug
      var response = new ValidationResponse{
        Valid = true,
        Data = doc,
        Contacts = contacts
      };

      return response;
    }).WithOpenApi(op => {
      op.Summary = "Validate the markdown for an email";
      op.Description = "Before you send a broadcast, ping this endpoint to ensure that the markdown is valid";
      op.RequestBody.Description = "The markdown for the email";
      return op;
    });
  }

}