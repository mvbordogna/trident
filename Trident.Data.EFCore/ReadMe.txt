DocumentDB.Spacial.Sql.dll and Microsoft.Azure.Doumcnets.ServiceInterop.dll
Are required by DocumentDB Client used by the EFCore Cosmos DB, 
at the present time, they are not automatically included in the project due to a bug in the preview 2 nuget package 
release of ef core 2.2.0 preview 2. Due to these packages being unmanaged code, then need to be dropped in the
root of the bin directory, probing paths do not work as of the time of this being written on 10/1/2018.

Also, since the release of the preview on 9/1/2018, the async api of the DBContext does work due ot a generic parameter check that goes 
out of index... This is only an issue for Cosmos and not Sql Server provider. There is a specific db context for Cosmos,
make sure to delete the method override implemenations after the next release of EFCore/EFCore.CosmosSQL provider. It has already
been fixed in the 2.2.0 release branch as of 9/29/2018.

The EFCore implementation for RA, based on changes on how ef core works in comparison to EF6, which now caches the model
builder results, as such, each model set must have there own db context. however multi-tenency assuming using the same model per tenant
database, only needs the connection string swapped out on a per instance bases. Also due to current needs, 
the TenantConnectionStringResolver is not implemented, and would need to be if this support is needed in the future.


https://github.com/aspnet/EntityFrameworkCore/issues/12086

