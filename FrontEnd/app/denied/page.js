"use client";
import { Suspense } from "react";
import { useRouter, useSearchParams } from "next/navigation";

function DeniedContent() {
  const router = useRouter();
  const params = useSearchParams();
  const reason = params.get("reason");

  return (
    <main className="result-shell result-denied">
      <div className="result-panel">
        <div className="result-icon" aria-hidden="true">!</div>
        <p className="eyebrow">NIURO / APPLICATION STATUS</p>
        <h1>Application not approved</h1>
        <p>{reason || "Your loan application did not meet the approval criteria."}</p>
        <button onClick={() => router.push("/")} className="result-button">
          Review your application <span aria-hidden="true">→</span>
        </button>
      </div>
    </main>
  );
}

export default function DeniedPage() {
  return (
    <Suspense fallback={<div className="p-6">Loading...</div>}>
      <DeniedContent />
    </Suspense>
  );
}
