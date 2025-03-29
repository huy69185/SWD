import { BaseApi, ApiConfig } from './baseApi';

class AdminApi extends BaseApi {
  constructor() {
    super();
    this.baseUrl = `${ApiConfig.getBaseUrl()}/admin`;
  }

  // User management
  async getAllUsers() {
    return this.get('/users');
  }
  
  async updateUserStatus(userId: string, status: string) {
    return this.patch(`/users/${userId}/status`, { status });
  }
  
  // Doctor verification
  async getPendingDoctors() {
    return this.get('/doctors/pending');
  }
  
  async verifyDoctor(doctorId: string, isApproved: boolean, remarks?: string) {
    return this.post(`/doctors/${doctorId}/verify`, { isApproved, remarks });
  }
  
  // Transaction management
  async getAllTransactions() {
    return this.get<Transaction[]>('/transactions');
  }
  
  // Refund management
  async getRefundRequests() {
    return this.get<RefundRequest[]>('/refunds/pending');
  }
  
  async approveRefund(refundId: string) {
    return this.post<boolean>(`/refunds/${refundId}/approve`, {});
  }
  
  async rejectRefund(refundId: string) {
    return this.post<boolean>(`/refunds/${refundId}/reject`, {});
  }
  
  // Feedback management
  async getAllFeedback() {
    return this.get<ConsultationFeedback[]>('/feedback');
  }
  
  async updateFeedbackStatus(feedbackId: string, status: FeedbackStatus) {
    return this.patch<boolean>(`/feedback/${feedbackId}/status`, { status });
  }
  
  // User reports
  async getUserReports() {
    return this.get<UserReport[]>('/reports');
  }
  
  async updateReportStatus(reportId: string, status: FeedbackStatus, adminResponse?: string) {
    return this.patch<boolean>(`/reports/${reportId}/status`, { 
      status,
      adminResponse
    });
  }
  
  // Bug reports
  async getBugReports() {
    return this.get('/bugs');
  }
  
  async updateBugStatus(bugId: string, status: string) {
    return this.patch(`/bugs/${bugId}`, { status });
  }
}

export const adminApi = new AdminApi();
