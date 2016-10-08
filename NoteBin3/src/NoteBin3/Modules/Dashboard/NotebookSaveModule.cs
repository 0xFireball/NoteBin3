using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using NoteBin3.Models.Data;
using NoteBin3.Services.Authentication;
using NoteBin3.Services.UserData;
using NoteBin3.Types;
using System;

namespace NoteBin3.Modules.Dashboard
{
    /// <summary>
    /// A module to help save the user's data
    /// </summary>
    public class NotebookSaveModule : NancyModule
    {
        public NotebookSaveModule()
        {
            this.RequiresAuthentication();
            Post("/dashboard/notebook/{id}/save", args =>
            {
                //bind the data to a model and save it
                var dataPackage = this.Bind<NotebookSaveDataModel>();

                int epoch = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                Guid notebookGuid;
                var validInputGuid = Guid.TryParse(args.id, out notebookGuid);
                if (!validInputGuid)
                {
                    return new Response
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ReasonPhrase = "Id was invalid"
                    };
                }

                var nbData = new NotebookData
                {
                    MarkdownText = dataPackage.Contents,
                };

                var userManagerConnection = new WebLoginUserManager();

                var currentUser = userManagerConnection.FindUserByUsername(Context.CurrentUser.Identity.Name);

                var notebookAccessConnection = new NotebookAccessManager();
                var currentNotebook = notebookAccessConnection.LoadOrCreateNotebook(currentUser, notebookGuid, userManagerConnection);

                //Update notebook data
                currentNotebook.Contents = nbData;

                return "OK";
            });
        }
    }
}