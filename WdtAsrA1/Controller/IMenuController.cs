using System.Text;

namespace WdtAsrA1.Controller
 {
     internal interface IMenuController
     {
         string MenuHeader { get; set; }
         BaseController Parent { get; }
         int BuildMenu(out StringBuilder menu);
     }
 }