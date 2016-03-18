using System.Collections.Generic;

public class ActionStack
{
   private LinkedList<IEditAction> _actions;
   private LinkedList<IEditAction>.Enumerator _currentAction;

   public ActionStack()
   {
      _actions = new LinkedList<IEditAction>();
      _currentAction = _actions.GetEnumerator();
   }

   public void AddAction( IEditAction action )
   {
   }

   public bool CanUndo()
   {
      return true;
   }

   public bool CanRedo()
   {
      return true;
   }

   public void Undo()
   {
   }

   public void Redo()
   {
   }
}
