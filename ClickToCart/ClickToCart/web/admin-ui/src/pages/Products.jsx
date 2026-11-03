import { useState } from "react";
import { Copy, Megaphone } from "lucide-react";
import { api } from "../api.js";
import { useAsync } from "../hooks.js";
import { Empty, ErrorBox, Loading, Modal, PageHeader, useToast } from "../components.jsx";
import { money } from "../format.js";

export default function Products() {
  const { data, error, loading, reload } = useAsync(api.products);
  const [selected, setSelected] = useState(null);
  if (loading && !data) return <Loading />;
  if (error) return <ErrorBox error={error} retry={reload} />;

  return (
    <>
      <PageHeader title="Products" subtitle="Pick a product and turn it into a trackable Facebook or Instagram ad link." />
      <section className="grid">
        {data.map((p) => (
          <article key={p.id} className="card product">
            <div className="thumb">{p.name[0]}</div>
            <h3>{p.name}</h3>
            <p className="price">{money(p.price)}</p>
            <button className="btn accent" onClick={() => setSelected(p)}><Megaphone size={16} /> Promote on Facebook</button>
          </article>
        ))}
      </section>
      {selected && <PromoteModal product={selected} onClose={() => setSelected(null)} />}
    </>
  );
}

function PromoteModal({ product, onClose }) {
  const toast = useToast();
  const { data: campaigns } = useAsync(api.campaigns);
  const active = (campaigns ?? []).filter((c) => c.status === "Active");
  const [form, setForm] = useState({ campaignId: "", channel: "facebook", adName: "video_ad" });
  const [result, setResult] = useState(null);
  const set = (key) => (e) => setForm({ ...form, [key]: e.target.value });

  async function submit(e) {
    e.preventDefault();
    try {
      setResult(await api.createLink({ ...form, campaignId: form.campaignId || active[0]?.id, productId: product.id }));
    } catch (err) {
      toast(err.message, "bad");
    }
  }

  async function copy() {
    await navigator.clipboard.writeText(result.shortUrl);
    toast("Link copied");
  }

  return (
    <Modal title={`Promote ${product.name}`} onClose={onClose}>
      {result ? (
        <div className="result">
          <p className="muted">Paste this in the Website URL field of your ad.</p>
          <div className="short-url">{result.shortUrl}</div>
          <div className="actions">
            <button className="btn" onClick={copy}><Copy size={16} /> Copy link</button>
            <button className="btn ghost" onClick={onClose}>Done</button>
          </div>
        </div>
      ) : active.length === 0 ? (
        <Empty>No active campaign. Activate one on the Campaigns page first.</Empty>
      ) : (
        <form onSubmit={submit}>
          <label>Campaign
            <select value={form.campaignId || active[0].id} onChange={set("campaignId")}>
              {active.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
            </select>
          </label>
          <label>Channel
            <select value={form.channel} onChange={set("channel")}>
              <option value="facebook">Facebook</option>
              <option value="instagram">Instagram</option>
            </select>
          </label>
          <label>Ad name
            <input value={form.adName} onChange={set("adName")} required />
          </label>
          <button className="btn accent" type="submit">Generate link</button>
        </form>
      )}
    </Modal>
  );
}
