using Application;
using Application.BaseManager;
using AutoMapper;
namespace SkyLearn.Portal.Api.Services
{
    public class PageActionService : RestService
    {
        private readonly Context _context;
        public PageActionService(Context context, IMapper mapper) : base(context, mapper)   
        {
            _context = context;
        }
        public async Task AutomateActions(ControllerAction action)
        {
            var existingAction = _context.ControllerActions
                .FirstOrDefault(a => a.Area == action.Area && a.Controller == action.Controller && a.Action == action.Action);

            if (existingAction == null)
            {
                _context.ControllerActions.Add(action);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _context.ControllerActions.Remove(action);
                }
            }

        }
    }
}
