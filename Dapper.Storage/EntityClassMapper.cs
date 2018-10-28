using System;
using System.Reflection;
using DapperExtensions.Mapper;

namespace Dapper.Storage
{
	using Table = Attributes.TableAttribute;
	using Key = Attributes.KeyAttribute;
	using CompositeKey = Attributes.CompositeKeyAttribute;
	using Column = Attributes.ColumnAttribute;

	public static class AttributeHelper
	{
		public static bool HasAttribute<TAttribute>(this object source, out TAttribute attribute)
			where TAttribute : Attribute => HasAttribute(source.GetType(), out attribute);

		public static bool HasAttribute<TAttribute>(this MemberInfo member, out TAttribute attribute)
			where TAttribute : Attribute
		{
			attribute = member.GetCustomAttribute<TAttribute>();
			return attribute != null;
		}
	}

	public class EntityClassMapper<TEntity> : ClassMapper<TEntity>
		where TEntity : class
	{
		public EntityClassMapper()
		{
			this.MapEntity(EntityType);
		}

		private void MapEntity(Type entityType)
		{
			if(entityType.HasAttribute(out Table table))
			{
				Table(table.Name);
				Schema(table.Schema);
			}

			foreach (var property in entityType.GetProperties())
			{
				this.MapProperty(property);
			}
		}

		private void MapProperty(PropertyInfo property)
		{
			var map = Map(property);
			if (property.HasAttribute(out Key _))
			{
				map.Key(KeyType.Identity);
			}

			if (property.HasAttribute(out CompositeKey _))
			{
				map.Key(KeyType.Assigned);
			}

			if (property.HasAttribute(out Column column))
			{
				map.Column(column.Name);
			}
		}
	}
}
