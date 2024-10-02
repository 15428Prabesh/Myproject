interface RequestOptions{
    url:string
    method:string
    body:any
    headers?:Record<string,string>

}

export default RequestOptions