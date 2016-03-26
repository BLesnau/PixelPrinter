using System.Collections.Generic;

public class ActionStack
{
   private readonly LinkedList<IEditAction> _actions;
   private LinkedListNode<IEditAction> _currentAction;

   public ActionStack()
   {
      _actions = new LinkedList<IEditAction>();
      _currentAction = _actions.First;
   }

   public void AddAction( IEditAction action )
   {
      _actions.RemoveAllAfter( _currentAction );

      if ( _currentAction != null )
      {
         _currentAction = _actions.AddAfter( _currentAction, action );
      }
      else
      {
         _currentAction = _actions.AddFirst( action );
      }
   }

   public bool CanUndo()
   {
      return _currentAction != null;
   }

   public bool CanRedo()
   {
      if ( _currentAction != null )
      {
         return _currentAction.Next != null;
      }

      return false;
   }

   public void Undo()
   {
      _currentAction.Value.Undo();
      _currentAction = _currentAction.Previous;
   }

   public void Redo()
   {
      _currentAction = _currentAction.Next;
      _currentAction.Value.Redo();
   }
}
