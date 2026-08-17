"use client";
import { useRouter } from "next/navigation";

export default function ApprovedPage() {
  const router = useRouter();

  return (
    <div className="flex items-center justify-center min-h-screen bg-green-100">
      <div className="max-w-md w-full p-6 bg-green-600 text-white shadow rounded text-center">
        <h1 className="text-2xl font-bold mb-4">Application Approved ✅</h1>
        <p className="text-lg mb-6">Your loan application has been successfully approved.</p>
        <button
          onClick={() => router.push("/")}
          className="bg-white text-green-700 px-4 py-2 rounded"
        >
          Back to Home
        </button>
      </div>
    </div>
  );
}
