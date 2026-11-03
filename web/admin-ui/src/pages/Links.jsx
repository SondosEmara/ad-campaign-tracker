import { Copy } from "lucide-react";
import { useCallback } from "react";
import { api } from "../api.js";
import { useAsync } from "../hooks.js";
import { Badge, Empty, ErrorBox, Loading, PageHeader, useToast } from "../components.jsx";

const loadAll = () => Promise.all([api.links(), api.campaigns()]);

export default function Links() {
  const { data, error, loading, reload } = useAsync(loadAll);
  const toast = useToast();
  if (loading && !data) return <Loading />;
  if (error) return <ErrorBox error={error} retry={reload} />;

  const [links, campaigns] = data;
  const campaignName = Object.fromEntries(campaigns.map((c) => [c.id, c.name]));

  async function copy(url) {
    await navigator.clipboard.writeText(url);
    toast("Link copied");
  }

  async function disable(code) {
    try {
      await api.disableLink(code);
      toast("Link disabled. Clicks now go to the home page.");
      reload();
    } catch (err) {
      toast(err.message, "bad");
    }
  }

  return (
    <>
      <PageHeader title="Short links" subtitle="One link per ad, so you can tell which ad sells." />
      <section className="card">
        {links.length === 0 ? <Empty>No links yet.</Empty> : (
          <div className="table-wrap"><table>
            <thead><tr><th>Short link</th><th>Ad</th><th>Campaign</th><th>Product</th><th>Status</th><th /></tr></thead>
            <tbody>
              {links.map((l) => (
                <tr key={l.code}>
                  <td className="mono">{l.shortUrl}
                    <button className="icon-btn" onClick={() => copy(l.shortUrl)} aria-label="Copy link"><Copy size={15} /></button>
                  </td>
                  <td>{l.adName} <span className="muted small">({l.channel})</span></td>
                  <td>{campaignName[l.campaignId] ?? "-"}</td>
                  <td>{l.productName}</td>
                  <td><Badge tone={l.isEnabled ? "good" : "bad"}>{l.isEnabled ? "Enabled" : "Disabled"}</Badge></td>
                  <td className="row-actions">
                    {l.isEnabled && <button className="btn small ghost" onClick={() => disable(l.code)}>Disable</button>}
                  </td>
                </tr>
              ))}
            </tbody>
          </table></div>
        )}
      </section>
    </>
  );
}
