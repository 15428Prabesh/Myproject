using System.ComponentModel.DataAnnotations.Schema;
using Application;
using Application.Models;

public class ActionPermission : BaseModel
{
    public int ControllerActionId { get; set; }
    [ForeignKey("ControllerActionId")]
    public virtual ControllerAction ControllerAction { get; set; }

    public int RoleId { get; set; }
    [ForeignKey("RoleId")]
    public virtual Roles Roles { get; set; }
}


