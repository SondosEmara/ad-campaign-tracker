import { api } from "../api.js";
import { useAsync } from "../hooks.js";
import { Badge, Empty, ErrorBox, Loading, PageHeader } from "../components.jsx";
import { money } from "../format.js";

const loadAll = () => Promise.all([api.orders(), api.links(), api.campaigns()]);

export default function Orders() {
  const { data, error, loading, reload } = useAsync(loadAll);
  if (loading && !data) return <Loading />;
  if (error) return <ErrorBox error={error} retry={reload} />;

  const [orders, links, campaigns] = data;
  const campaignName = Object.fromEntries(campaigns.map((c) => [c.id, c.name]));
  const source = Object.fromEntries(links.map((l) => [l.code, `${campaignName[l.campaignId] ?? "Campaign"} / ${l.adName}`]));

  return (
    <>
      <PageHeader title="Orders" subtitle="The “Came from” column shows which ad brought the order." />
      <section className="card">
        {orders.length === 0 ? <Empty>No orders yet.</Empty> : (
          <div className="table-wrap"><table>
            <thead><tr><th>Order</th><th>Product</th><th className="n">Qty</th><th className="n">Total</th><th>Came from</th><th>Date</th></tr></thead>
            <tbody>
              {orders.map((o) => (
                <tr key={o.id}>
                  <td className="mono">{o.id}</td>
                  <td>{o.productName}</td>
                  <td className="n">{o.quantity}</td>
                  <td className="n">{money(o.total)}</td>
                  <td>{o.linkCode ? <Badge tone="accent">{source[o.linkCode] ?? o.linkCode}</Badge> : <span className="muted">Normal (no ad)</span>}</td>
                  <td className="muted">{new Date(o.createdAt).toLocaleDateString()}</td>
                </tr>
              ))}
            </tbody>
          </table></div>
        )}
      </section>
    </>
  );
}
