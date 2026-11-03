import { RefreshCw } from "lucide-react";
import { api } from "../api.js";
import { useAsync } from "../hooks.js";
import { Badge, Empty, ErrorBox, Kpi, Loading, PageHeader } from "../components.jsx";
import { money, number, percent, ratio, roasTone } from "../format.js";

export default function Dashboard() {
  const { data, error, loading, reload } = useAsync(api.dashboard);
  if (loading && !data) return <Loading />;
  if (error) return <ErrorBox error={error} retry={reload} />;

  const { totals, links } = data;
  const maxRoas = Math.max(1, ...links.map((l) => l.roas));
  return (
    <>
      <PageHeader title="Which ad makes money?" subtitle="Revenue, spend and ROAS for every ad link.">
        <button className="btn ghost" onClick={reload}><RefreshCw size={16} /> Refresh</button>
      </PageHeader>

      <section className="kpis">
        <Kpi label="Revenue from ads" value={money(totals.revenue)} />
        <Kpi label="Ad spend" value={money(totals.spend)} />
        <Kpi label="ROAS" value={ratio(totals.roas)} tone={roasTone(totals.roas)} />
        <Kpi label="Orders" value={number(totals.orders)} />
        <Kpi label="Clicks" value={number(totals.clicks)} />
        <Kpi label="Cost per order" value={money(totals.cpa)} />
        <Kpi label="Click to order" value={percent(totals.conversionRate)} />
      </section>

      <section className="card">
        <h2>Ad performance</h2>
        {links.length === 0 ? <Empty>No links yet. Promote a product to create one.</Empty> : (
          <div className="table-wrap">
            <table>
              <thead><tr>
                <th>Ad</th><th>Product</th><th className="n">Clicks</th><th className="n">Orders</th>
                <th className="n">Revenue</th><th className="n">Spend</th><th>ROAS</th>
              </tr></thead>
              <tbody>
                {links.map((l) => (
                  <tr key={l.code}>
                    <td><strong>{l.adName}</strong><div className="muted small">{l.campaignName} · {l.channel}</div></td>
                    <td>{l.productName}</td>
                    <td className="n">{number(l.clicks)}</td>
                    <td className="n">{number(l.orders)}</td>
                    <td className="n">{money(l.revenue)}</td>
                    <td className="n">{money(l.spend)}</td>
                    <td className="roas-cell">
                      <div className="bar"><span className={roasTone(l.roas)} style={{ width: `${(l.roas / maxRoas) * 100}%` }} /></div>
                      <Badge tone={roasTone(l.roas)}>{ratio(l.roas)}</Badge>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
        <p className="muted small">ROAS = revenue ÷ spend. Green is 3 or more, red is below 1. Spend refreshes every 30 seconds.</p>
      </section>
    </>
  );
}
