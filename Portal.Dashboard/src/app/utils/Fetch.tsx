import RequestOptions from '../interface/RequestOptions'
import { BASE_URL} from './Endpoints'

async function Fetch({url,method,body,headers}:RequestOptions) {

    const res = await fetch(BASE_URL+url,{
        method:method,
        body:body,
        headers:headers
    })
   
    if (!res.ok) {
      throw new Error('Failed to fetch data')
    }
   
    return res.json()
  }


export default Fetch