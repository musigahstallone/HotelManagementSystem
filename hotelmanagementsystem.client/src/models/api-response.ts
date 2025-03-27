export class ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;

  constructor(success: boolean, message?: string, data?: T) {
    this.success = success;
    this.message = message;
    this.data = data;
  }

  static successResponse<T>(data: T, message: string = "Request successful"): ApiResponse<T> {
    return new ApiResponse<T>(true, message, data);
  }

  static failureResponse<T>(message: string): ApiResponse<T> {
    return new ApiResponse<T>(false, message);
  }
}

export class PaginatedResponse<T> extends ApiResponse<T[]> {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;

  constructor(data: T[], page: number, pageSize: number, totalCount: number, message: string = "Request successful") {
    super(true, message, data);
    this.page = page;
    this.pageSize = pageSize;
    this.totalCount = totalCount;
    this.totalPages = Math.ceil(totalCount / pageSize);
  }
}

