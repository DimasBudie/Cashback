using Flunt.Notifications;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Cashback.Middlewares
{
    public abstract class Request<TResponse> : Notifiable, IRequest<TResponse>
    {
    }

    public class Response
    {
        private List<Notification> _notifications { get; } = new List<Notification>();

        public IReadOnlyCollection<Notification> Notifications => _notifications.ToList();

        public bool Invalid => _notifications.Any();

        public object Value { get; private set; }
        public Response()
        {
        }

        public Response(object @object) : this()
        {
            AddValue(@object);
        }

        public void AddValue(object @object)
        {
            Value = @object;
        }

        public void AddNotification(string message)
        {
            _notifications.Add(new Notification(null, message));
        }

        public void AddNotifications(IEnumerable<Notification> notificacoes)
        {
            _notifications.AddRange(notificacoes);
        }

        public IEnumerable<ProblemDetails> GetProblemDetails(Response response)
        {
            return response.Notifications.Select(item => new ProblemDetails
            {
                Title = item.Message,
                Status = StatusCodes.Status409Conflict,
                Detail = item.Message,
            });
        }
    }

    
}