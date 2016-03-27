using System.Collections.Generic;
using System.Linq;

public class ActionStack
{
   private readonly LinkedList<IEditAction> _actions;
   private LinkedListNode<IEditAction> _currentAction;

   public ActionStack()
   {
      _actions = new LinkedList<IEditAction>();
      _actions.AddFirst( new DummyAction() );
      _currentAction = _actions.First;
   }

   public void AddAction( IEditAction action )
   {
      _actions.RemoveAllAfter( _currentAction );
      _currentAction = _actions.AddAfter( _currentAction, action );
   }

   public bool CanUndo()
   {
      return !(_currentAction.Value is DummyAction);
   }

   public bool CanRedo()
   {
      return _currentAction.Next != null;
   }

   public void Undo()
   {
      if ( CanUndo() )
      {
         _currentAction.Value.Undo();
         _currentAction = _currentAction.Previous;
      }
   }

   public void Redo()
   {
      if ( CanRedo() )
      {
         _currentAction = _currentAction.Next;
         _currentAction.Value.Redo();
      }
   }
}
