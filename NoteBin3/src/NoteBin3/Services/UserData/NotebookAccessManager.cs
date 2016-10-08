using NoteBin3.Services.Authentication;
using NoteBin3.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NoteBin3.Services.UserData
{
    public class NotebookAccessManager
    {
        public Notebook LoadNotebook(RegisteredUser currentUser, Guid notebookGuid)
        {
            Notebook loadedNotebook = currentUser.Notebooks.FirstOrDefault(n => n.Identifier == notebookGuid);
            return loadedNotebook;
        }

        /// <summary>
        /// Load an existing notebook, or create one if it doesn't exist.
        /// </summary>
        /// <param name="currentUser">The current user</param>
        /// <param name="notebookGuid">The GUID to look for, or to make a new notebook with</param>
        /// <returns></returns>
        public Notebook LoadOrCreateNotebook(RegisteredUser currentUser, Guid notebookGuid)
        {
            //load existing notebook if available
            var loadedNotebook = LoadNotebook(currentUser, notebookGuid);
            if (loadedNotebook == null)
            {
                if (currentUser.Notebooks == null)
                {
                    currentUser.Notebooks = new List<Notebook>();
                }
                //make new notebook
                var newNotebook = new Notebook
                {
                    Contents = new NotebookData
                    {
                        MarkdownText = string.Empty
                    },
                    Identifier = notebookGuid
                };

                //save new notebook
                currentUser.Notebooks.Add(newNotebook);

                //update reference to point to new notebook
                loadedNotebook = newNotebook;
            }

            return loadedNotebook;
        }
    }
}