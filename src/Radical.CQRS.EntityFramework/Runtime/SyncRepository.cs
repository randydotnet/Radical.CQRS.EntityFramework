﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Newtonsoft.Json;
using Radical.CQRS.Reflection;
using Topics.Radical.Linq;
using System.Threading.Tasks;
using Radical.CQRS.Services;
using Radical.CQRS.Data;

namespace Radical.CQRS.Runtime
{
	class SyncRepository : AbstractSyncRepository<DomainContext, EntityFrameworkDomainEventCommit>
	{
		public override void Dispose()
		{
			base.Dispose();

			this.session.Dispose();
		}
		
		public SyncRepository( DomainContext session, IAggregateFinderProvider<DomainContext> aggregateFinderProvider, IAggregateStateFinderProvider<DomainContext> aggregateStateFinderProvider )
			: base( session, aggregateFinderProvider, aggregateStateFinderProvider )
		{

		}

		//public override void CommitChanges()
		//{
		//	try
		//	{
		//		var db = this._session.Set<DomainEventCommit>();

		//		this.AggregateTracking
		//			.Where( a => a.IsChanged )
		//			.Select( aggregate => new
		//			{
		//				Aggregate = aggregate,
		//				Commits = aggregate.GetUncommittedEvents().Select( e => new EntityFrameworkDomainEventCommit()
		//				{
		//					EventId = e.Id,
		//					AggregateId = aggregate.Id,
		//					Version = e.AggregateVersion,
		//					TransactionId = this.TransactionId,
		//					PublishedOn = e.OccurredAt,
		//					Event = e,
		//				} )
		//			} )
		//			.SelectMany( a => a.Commits )
		//			.ToArray()
		//			.ForEach( temp =>
		//			{
		//				db.Add( temp );
		//			} );

		//		this._session.SaveChanges();

		//		this.AggregateTracking.ForEach( a => a.ClearUncommittedEvents() );
		//		this.AggregateTracking.Clear();

		//	}
		//	catch( Exception ex )
		//	{
		//		//TODO: log
		//		throw;
		//	}
		//}

		//public override TAggregate GetById<TAggregate>( Guid aggregateId )
		//{
		//	TAggregate aggregate = null;
		//	var loader = this.aggregateLoaderProvider.GetLoader<TAggregate>();
		//	if( loader != null )
		//	{
		//		aggregate = loader.GetById( this._session, aggregateId );
		//	}
		//	else
		//	{
		//		var db = this._session.Set<TAggregate>();
		//		aggregate = db.Single( a => a.Id == aggregateId );
		//	}

		//	this.TrackIfRequired( aggregate );

		//	return aggregate;
		//}

		//public override IEnumerable<TAggregate> GetById<TAggregate>( params Guid[] aggregateIds )
		//{
		//	IEnumerable<TAggregate> results = null;
		//	var loader = this.aggregateLoaderProvider.GetLoader<TAggregate>();
		//	if( loader != null )
		//	{
		//		results = loader.GetById( this._session, aggregateIds );
		//	}
		//	else
		//	{
		//		var db = this._session.Set<TAggregate>();
		//		results = db.Where( a => aggregateIds.Contains( a.Id ) )
		//			.ToList();
		//	}

		//	foreach( var a in results )
		//	{
		//		this.TrackIfRequired( a );
		//	}

		//	return results;
		//}

		protected override void OnCommitChanges()
		{
			this.session.SaveChanges();
		}

		protected override void OnAdd( object aggregateOrState )
		{
			var db = this.session.Set( aggregateOrState.GetType() );
			db.Add( aggregateOrState );
		}

		protected override void OnAdd( IEnumerable<EntityFrameworkDomainEventCommit> commits )
		{
			var db = this.session.Set<EntityFrameworkDomainEventCommit>();
			foreach( var commit in commits )
			{
				db.Add( commit );
			}
		}
	}
}
